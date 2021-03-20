using Google.Protobuf;
using Google.Protobuf.Reflection;
using Google.Protobuf.WellKnownTypes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace DotBPE.BestPractice.AuditLog
{
    /// <summary>
    /// Reflection-based converter from messages to JSON.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Instances of this class are thread-safe, with no mutable state.
    /// </para>
    /// <para>
    /// This is a simple start to get JSON formatting working. As it's reflection-based,
    /// it's not as quick as baking calls into generated messages - but is a simpler implementation.
    /// (This code is generally not heavily optimized.)
    /// </para>
    /// </remarks>
    public sealed class AuditJsonFormatter
    {
        public const string AnyTypeUrlField = "@type";
        public const string AnyDiagnosticValueField = "@value";
        public const string AnyWellKnownTypeValueField = "value";
        private const string TypeUrlPrefix = "type.googleapis.com";
        private const string NameValueSeparator = ": ";
        private const string PropertySeparator = ", ";

        /// <summary>
        /// Returns a formatter using the default settings.
        /// </summary>
        public static AuditJsonFormatter Default { get; } = new AuditJsonFormatter(Settings.Default);


        /// <summary>
        /// The JSON representation of the first 160 characters of Unicode.
        /// Empty strings are replaced by the static constructor.
        /// </summary>
        private static readonly string[] CommonRepresentations = {
            // C0 (ASCII and derivatives) control characters
            "\\u0000", "\\u0001", "\\u0002", "\\u0003",  // 0x00
          "\\u0004", "\\u0005", "\\u0006", "\\u0007",
          "\\b",     "\\t",     "\\n",     "\\u000b",
          "\\f",     "\\r",     "\\u000e", "\\u000f",
          "\\u0010", "\\u0011", "\\u0012", "\\u0013",  // 0x10
          "\\u0014", "\\u0015", "\\u0016", "\\u0017",
          "\\u0018", "\\u0019", "\\u001a", "\\u001b",
          "\\u001c", "\\u001d", "\\u001e", "\\u001f",
            // Escaping of " and \ are required by www.json.org string definition.
            // Escaping of < and > are required for HTML security.
            "", "", "\\\"", "", "",        "", "",        "",  // 0x20
          "", "", "",     "", "",        "", "",        "",
          "", "", "",     "", "",        "", "",        "",  // 0x30
          "", "", "",     "", "\\u003c", "", "\\u003e", "",
          "", "", "",     "", "",        "", "",        "",  // 0x40
          "", "", "",     "", "",        "", "",        "",
          "", "", "",     "", "",        "", "",        "",  // 0x50
          "", "", "",     "", "\\\\",    "", "",        "",
          "", "", "",     "", "",        "", "",        "",  // 0x60
          "", "", "",     "", "",        "", "",        "",
          "", "", "",     "", "",        "", "",        "",  // 0x70
          "", "", "",     "", "",        "", "",        "\\u007f",
            // C1 (ISO 8859 and Unicode) extended control characters
            "\\u0080", "\\u0081", "\\u0082", "\\u0083",  // 0x80
          "\\u0084", "\\u0085", "\\u0086", "\\u0087",
          "\\u0088", "\\u0089", "\\u008a", "\\u008b",
          "\\u008c", "\\u008d", "\\u008e", "\\u008f",
          "\\u0090", "\\u0091", "\\u0092", "\\u0093",  // 0x90
          "\\u0094", "\\u0095", "\\u0096", "\\u0097",
          "\\u0098", "\\u0099", "\\u009a", "\\u009b",
          "\\u009c", "\\u009d", "\\u009e", "\\u009f"
        };

        static AuditJsonFormatter()
        {
            for (int i = 0; i < CommonRepresentations.Length; i++)
            {
                if (CommonRepresentations[i] == "")
                {
                    CommonRepresentations[i] = ((char)i).ToString();
                }
            }
        }

        private readonly Settings settings;


        /// <summary>
        /// Creates a new formatted with the given settings.
        /// </summary>
        /// <param name="settings">The settings.</param>
        public AuditJsonFormatter(Settings settings)
        {
            this.settings = settings;
        }

        /// <summary>
        /// Formats the specified message as JSON.
        /// </summary>
        /// <param name="message">The message to format.</param>
        /// <returns>The formatted message.</returns>
        public string Format(IMessage message)
        {
            var writer = new StringWriter();
            Format(message, writer);
            return writer.ToString();
        }

        /// <summary>
        /// Formats the specified message as JSON.
        /// </summary>
        /// <param name="message">The message to format.</param>
        /// <param name="writer">The TextWriter to write the formatted message to.</param>
        /// <returns>The formatted message.</returns>
        public void Format(IMessage message, TextWriter writer)
        {
            ProtoPreconditions.CheckNotNull(message, nameof(message));
            ProtoPreconditions.CheckNotNull(writer, nameof(writer));

            WriteMessage(writer, message);
        }

        private void WriteMessage(TextWriter writer, IMessage message)
        {
            if (message == null)
            {
                WriteNull(writer);
                return;
            }

            writer.Write("{ ");
            bool writtenFields = WriteMessageFields(writer, message, false);
            writer.Write(writtenFields ? " }" : "}");
        }

        private bool WriteMessageFields(TextWriter writer, IMessage message, bool assumeFirstFieldWritten)
        {
            var fields = message.Descriptor.Fields;
            bool first = !assumeFirstFieldWritten;
            // First non-oneof fields
            foreach (var field in fields.InFieldNumberOrder())
            {
                var accessor = field.Accessor;
                if (field.ContainingOneof != null && field.ContainingOneof.Accessor.GetCaseFieldDescriptor(message) != field)
                {
                    continue;
                }
                // Omit default values unless we're asked to format them, or they're oneofs (where the default
                // value is still formatted regardless, because that's how we preserve the oneof case).
                object value = accessor.GetValue(message);
                if (field.ContainingOneof == null && !settings.FormatDefaultValues && IsDefaultValue(accessor, value))
                {
                    continue;
                }

                // Okay, all tests complete: let's write the field value...
                if (!first)
                {
                    writer.Write(PropertySeparator);
                }

                WriteString(writer, accessor.Descriptor.JsonName);
                writer.Write(NameValueSeparator);
                //处理数据字段脱敏
                if (CheckInMaskField(accessor.Descriptor.JsonName))
                {
                    WriteValue(writer, "******");
                }
                else
                {
                    WriteValue(writer, value);
                }

                first = false;
            }
            return !first;
        }

        private bool CheckInMaskField(string jsonName)
        {
            return ProtocolsConstants.FieldMaskList.Contains(jsonName.ToLower());
        }

        // Converted from java/core/src/main/java/com/google/protobuf/Descriptors.java
        public static string ToJsonName(string name)
        {
            StringBuilder result = new StringBuilder(name.Length);
            bool isNextUpperCase = false;
            foreach (char ch in name)
            {
                if (ch == '_')
                {
                    isNextUpperCase = true;
                }
                else if (isNextUpperCase)
                {
                    result.Append(char.ToUpperInvariant(ch));
                    isNextUpperCase = false;
                }
                else
                {
                    result.Append(ch);
                }
            }
            return result.ToString();
        }

        private static void WriteNull(TextWriter writer)
        {
            writer.Write("null");
        }

        private static bool IsDefaultValue(IFieldAccessor accessor, object value)
        {
            if (accessor.Descriptor.IsMap)
            {
                IDictionary dictionary = (IDictionary)value;
                return dictionary.Count == 0;
            }
            if (accessor.Descriptor.IsRepeated)
            {
                IList list = (IList)value;
                return list.Count == 0;
            }
            switch (accessor.Descriptor.FieldType)
            {
                case FieldType.Bool:
                    return (bool)value == false;

                case FieldType.Bytes:
                    return (ByteString)value == ByteString.Empty;

                case FieldType.String:
                    return (string)value == "";

                case FieldType.Double:
                    return (double)value == 0.0;

                case FieldType.SInt32:
                case FieldType.Int32:
                case FieldType.SFixed32:
                case FieldType.Enum:
                    return (int)value == 0;

                case FieldType.Fixed32:
                case FieldType.UInt32:
                    return (uint)value == 0;

                case FieldType.Fixed64:
                case FieldType.UInt64:
                    return (ulong)value == 0;

                case FieldType.SFixed64:
                case FieldType.Int64:
                case FieldType.SInt64:
                    return (long)value == 0;

                case FieldType.Float:
                    return (float)value == 0f;

                case FieldType.Message:
                case FieldType.Group: // Never expect to get this, but...
                    return value == null;

                default:
                    throw new ArgumentException("Invalid field type");
            }
        }

        /// <summary>
        /// Writes a single value to the given writer as JSON. Only types understood by
        /// Protocol Buffers can be written in this way. This method is only exposed for
        /// advanced use cases; most users should be using <see cref="Format(IMessage)"/>
        /// or <see cref="Format(IMessage, TextWriter)"/>.
        /// </summary>
        /// <param name="writer">The writer to write the value to. Must not be null.</param>
        /// <param name="value">The value to write. May be null.</param>
        public void WriteValue(TextWriter writer, object value)
        {
            if (value == null)
            {
                WriteNull(writer);
            }
            else if (value is bool)
            {
                writer.Write((bool)value ? "true" : "false");
            }
            else if (value is ByteString)
            {
                // Nothing in Base64 needs escaping
                writer.Write('"');
                writer.Write(((ByteString)value).ToBase64());
                writer.Write('"');
            }
            else if (value is string)
            {
                WriteString(writer, (string)value);
            }
            else if (value is IDictionary)
            {
                WriteDictionary(writer, (IDictionary)value);
            }
            else if (value is IList)
            {
                WriteList(writer, (IList)value);
            }
            else if (value is int || value is uint)
            {
                IFormattable formattable = (IFormattable)value;
                writer.Write(formattable.ToString("d", CultureInfo.InvariantCulture));
            }
            else if (value is long || value is ulong)
            {
                writer.Write('"');
                IFormattable formattable = (IFormattable)value;
                writer.Write(formattable.ToString("d", CultureInfo.InvariantCulture));
                writer.Write('"');
            }
            else if (value is System.Enum)
            {
                if (settings.FormatEnumsAsIntegers)
                {
                    WriteValue(writer, (int)value);
                }
                else
                {
                    string name = OriginalEnumValueHelper.GetOriginalName(value);
                    if (name != null)
                    {
                        WriteString(writer, name);
                    }
                    else
                    {
                        WriteValue(writer, (int)value);
                    }
                }
            }
            else if (value is float || value is double)
            {
                string text = ((IFormattable)value).ToString("r", CultureInfo.InvariantCulture);
                if (text == "NaN" || text == "Infinity" || text == "-Infinity")
                {
                    writer.Write('"');
                    writer.Write(text);
                    writer.Write('"');
                }
                else
                {
                    writer.Write(text);
                }
            }
            else if (value is IMessage)
            {
                Format((IMessage)value, writer);
            }
            else
            {
                throw new ArgumentException("Unable to format value of type " + value.GetType());
            }
        }

        private void WriteDiagnosticOnlyAny(TextWriter writer, IMessage value)
        {
            string typeUrl = (string)value.Descriptor.Fields[Any.TypeUrlFieldNumber].Accessor.GetValue(value);
            ByteString data = (ByteString)value.Descriptor.Fields[Any.ValueFieldNumber].Accessor.GetValue(value);
            writer.Write("{ ");
            WriteString(writer, AnyTypeUrlField);
            writer.Write(NameValueSeparator);
            WriteString(writer, typeUrl);
            writer.Write(PropertySeparator);
            WriteString(writer, AnyDiagnosticValueField);
            writer.Write(NameValueSeparator);
            writer.Write('"');
            writer.Write(data.ToBase64());
            writer.Write('"');
            writer.Write(" }");
        }

        /// <summary>
        /// 重写写入list的代码，只记录记录调试
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="list">The list.</param>
        public void WriteList(TextWriter writer, IList list)
        {
            writer.Write("[ ");
            bool first = true;
            foreach (var value in list)
            {
                if (!first)
                {
                    writer.Write(PropertySeparator);
                }
                WriteValue(writer, value);
                first = false;
            }
            writer.Write(first ? "]" : " ]");
        }

        public void WriteDictionary(TextWriter writer, IDictionary dictionary)
        {
            writer.Write("{ ");
            bool first = true;
            // This will box each pair. Could use IDictionaryEnumerator, but that's ugly in terms of disposal.
            foreach (DictionaryEntry pair in dictionary)
            {
                if (!first)
                {
                    writer.Write(PropertySeparator);
                }
                string keyText;
                if (pair.Key is string)
                {
                    keyText = (string)pair.Key;
                }
                else if (pair.Key is bool)
                {
                    keyText = (bool)pair.Key ? "true" : "false";
                }
                else if (pair.Key is int || pair.Key is uint | pair.Key is long || pair.Key is ulong)
                {
                    keyText = ((IFormattable)pair.Key).ToString("d", CultureInfo.InvariantCulture);
                }
                else
                {
                    if (pair.Key == null)
                    {
                        throw new ArgumentException("Dictionary has entry with null key");
                    }
                    throw new ArgumentException("Unhandled dictionary key type: " + pair.Key.GetType());
                }
                WriteString(writer, keyText);
                writer.Write(NameValueSeparator);
                WriteValue(writer, pair.Value);
                first = false;
            }
            writer.Write(first ? "}" : " }");
        }

        /// <summary>
        /// 写入字符串，最长写入100个字符
        /// </summary>
        /// <remarks>
        /// Other than surrogate pair handling, this code is mostly taken from src/google/protobuf/util/public/json_escaping.cc.
        /// </remarks>
        public static void WriteString(TextWriter writer, string text)
        {
            writer.Write('"');
            for (int i = 0; i < text.Length && i < 100; i++)
            {
                char c = text[i];
                if (c < 0xa0)
                {
                    writer.Write(CommonRepresentations[c]);
                    continue;
                }
                if (char.IsHighSurrogate(c))
                {
                    // Encountered first part of a surrogate pair.
                    // Check that we have the whole pair, and encode both parts as hex.
                    i++;
                    if (i == text.Length || !char.IsLowSurrogate(text[i]))
                    {
                        throw new ArgumentException("String contains low surrogate not followed by high surrogate");
                    }
                    HexEncodeUtf16CodeUnit(writer, c);
                    HexEncodeUtf16CodeUnit(writer, text[i]);
                    continue;
                }
                else if (char.IsLowSurrogate(c))
                {
                    throw new ArgumentException("String contains high surrogate not preceded by low surrogate");
                }
                switch ((uint)c)
                {
                    // These are not required by json spec
                    // but used to prevent security bugs in javascript.
                    case 0xfeff:  // Zero width no-break space
                    case 0xfff9:  // Interlinear annotation anchor
                    case 0xfffa:  // Interlinear annotation separator
                    case 0xfffb:  // Interlinear annotation terminator

                    case 0x00ad:  // Soft-hyphen
                    case 0x06dd:  // Arabic end of ayah
                    case 0x070f:  // Syriac abbreviation mark
                    case 0x17b4:  // Khmer vowel inherent Aq
                    case 0x17b5:  // Khmer vowel inherent Aa
                        HexEncodeUtf16CodeUnit(writer, c);
                        break;

                    default:
                        if ((c >= 0x0600 && c <= 0x0603) ||  // Arabic signs
                            (c >= 0x200b && c <= 0x200f) ||  // Zero width etc.
                            (c >= 0x2028 && c <= 0x202e) ||  // Separators etc.
                            (c >= 0x2060 && c <= 0x2064) ||  // Invisible etc.
                            (c >= 0x206a && c <= 0x206f))
                        {
                            HexEncodeUtf16CodeUnit(writer, c);
                        }
                        else
                        {
                            // No handling of surrogates here - that's done earlier
                            writer.Write(c);
                        }
                        break;
                }
            }
            writer.Write('"');
        }

        private const string Hex = "0123456789abcdef";

        private static void HexEncodeUtf16CodeUnit(TextWriter writer, char c)
        {
            writer.Write("\\u");
            writer.Write(Hex[(c >> 12) & 0xf]);
            writer.Write(Hex[(c >> 8) & 0xf]);
            writer.Write(Hex[(c >> 4) & 0xf]);
            writer.Write(Hex[(c >> 0) & 0xf]);
        }

        /// <summary>
        /// Settings controlling JSON formatting.
        /// </summary>
        public sealed class Settings
        {
            /// <summary>
            /// Default settings, as used by <see cref="JsonFormatter.Default"/>
            /// </summary>
            public static Settings Default { get; }

            // Workaround for the Mono compiler complaining about XML comments not being on
            // valid language elements.
            static Settings()
            {
                Default = new Settings(false);
            }

            /// <summary>
            /// Whether fields whose values are the default for the field type (e.g. 0 for integers)
            /// should be formatted (true) or omitted (false).
            /// </summary>
            public bool FormatDefaultValues { get; }

            /// <summary>
            /// The type registry used to format <see cref="Any"/> messages.
            /// </summary>
            public TypeRegistry TypeRegistry { get; }

            /// <summary>
            /// Whether to format enums as ints. Defaults to false.
            /// </summary>
            public bool FormatEnumsAsIntegers { get; }

            /// <summary>
            /// Creates a new <see cref="Settings"/> object with the specified formatting of default values
            /// and an empty type registry.
            /// </summary>
            /// <param name="formatDefaultValues"><c>true</c> if default values (0, empty strings etc) should be formatted; <c>false</c> otherwise.</param>
            public Settings(bool formatDefaultValues) : this(formatDefaultValues, TypeRegistry.Empty)
            {
            }

            /// <summary>
            /// Creates a new <see cref="Settings"/> object with the specified formatting of default values
            /// and type registry.
            /// </summary>
            /// <param name="formatDefaultValues"><c>true</c> if default values (0, empty strings etc) should be formatted; <c>false</c> otherwise.</param>
            /// <param name="typeRegistry">The <see cref="TypeRegistry"/> to use when formatting <see cref="Any"/> messages.</param>
            public Settings(bool formatDefaultValues, TypeRegistry typeRegistry) : this(formatDefaultValues, typeRegistry, false)
            {
            }

            /// <summary>
            /// Creates a new <see cref="Settings"/> object with the specified parameters.
            /// </summary>
            /// <param name="formatDefaultValues"><c>true</c> if default values (0, empty strings etc) should be formatted; <c>false</c> otherwise.</param>
            /// <param name="typeRegistry">The <see cref="TypeRegistry"/> to use when formatting <see cref="Any"/> messages. TypeRegistry.Empty will be used if it is null.</param>
            /// <param name="formatEnumsAsIntegers"><c>true</c> to format the enums as integers; <c>false</c> to format enums as enum names.</param>
            private Settings(bool formatDefaultValues,
                            TypeRegistry typeRegistry,
                            bool formatEnumsAsIntegers)
            {
                FormatDefaultValues = formatDefaultValues;
                TypeRegistry = typeRegistry ?? TypeRegistry.Empty;
                FormatEnumsAsIntegers = formatEnumsAsIntegers;
            }

            /// <summary>
            /// Creates a new <see cref="Settings"/> object with the specified formatting of default values and the current settings.
            /// </summary>
            /// <param name="formatDefaultValues"><c>true</c> if default values (0, empty strings etc) should be formatted; <c>false</c> otherwise.</param>
            public Settings WithFormatDefaultValues(bool formatDefaultValues) => new Settings(formatDefaultValues, TypeRegistry, FormatEnumsAsIntegers);

            /// <summary>
            /// Creates a new <see cref="Settings"/> object with the specified type registry and the current settings.
            /// </summary>
            /// <param name="typeRegistry">The <see cref="TypeRegistry"/> to use when formatting <see cref="Any"/> messages.</param>
            public Settings WithTypeRegistry(TypeRegistry typeRegistry) => new Settings(FormatDefaultValues, typeRegistry, FormatEnumsAsIntegers);

            /// <summary>
            /// Creates a new <see cref="Settings"/> object with the specified enums formatting option and the current settings.
            /// </summary>
            /// <param name="formatEnumsAsIntegers"><c>true</c> to format the enums as integers; <c>false</c> to format enums as enum names.</param>
            public Settings WithFormatEnumsAsIntegers(bool formatEnumsAsIntegers) => new Settings(FormatDefaultValues, TypeRegistry, formatEnumsAsIntegers);
        }

        // Effectively a cache of mapping from enum values to the original name as specified in the proto file,
        // fetched by reflection.
        // The need for this is unfortunate, as is its unbounded size, but realistically it shouldn't cause issues.
        private static class OriginalEnumValueHelper
        {
            // TODO: In the future we might want to use ConcurrentDictionary, at the point where all
            // the platforms we target have it.
            private static readonly Dictionary<System.Type, Dictionary<object, string>> dictionaries
                = new Dictionary<System.Type, Dictionary<object, string>>();

            public static string GetOriginalName(object value)
            {
                var enumType = value.GetType();
                Dictionary<object, string> nameMapping;
                lock (dictionaries)
                {
                    if (!dictionaries.TryGetValue(enumType, out nameMapping))
                    {
                        nameMapping = GetNameMapping(enumType);
                        dictionaries[enumType] = nameMapping;
                    }
                }

                string originalName;
                // If this returns false, originalName will be null, which is what we want.
                nameMapping.TryGetValue(value, out originalName);
                return originalName;
            }

#if NET35
            // TODO: Consider adding functionality to TypeExtensions to avoid this difference.
            private static Dictionary<object, string> GetNameMapping(System.Type enumType) =>
                enumType.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static)
                    .Where(f => (f.GetCustomAttributes(typeof(OriginalNameAttribute), false)
                                 .FirstOrDefault() as OriginalNameAttribute)
                                 ?.PreferredAlias ?? true)
                    .ToDictionary(f => f.GetValue(null),
                                  f => (f.GetCustomAttributes(typeof(OriginalNameAttribute), false)
                                        .FirstOrDefault() as OriginalNameAttribute)
                                        // If the attribute hasn't been applied, fall back to the name of the field.
                                        ?.Name ?? f.Name);
#else

            private static Dictionary<object, string> GetNameMapping(System.Type enumType) =>
                enumType.GetTypeInfo().DeclaredFields
                    .Where(f => f.IsStatic)
                    .Where(f => f.GetCustomAttributes<OriginalNameAttribute>()
                                 .FirstOrDefault()?.PreferredAlias ?? true)
                    .ToDictionary(f => f.GetValue(null),
                                  f => f.GetCustomAttributes<OriginalNameAttribute>()
                                        .FirstOrDefault()
                                        // If the attribute hasn't been applied, fall back to the name of the field.
                                        ?.Name ?? f.Name);

#endif
        }
    }
}
