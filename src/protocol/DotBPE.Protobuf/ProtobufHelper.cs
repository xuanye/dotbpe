using Google.Protobuf;
using Google.Protobuf.Reflection;

namespace DotBPE.Protobuf
{
    public static class ProtobufHelper
    {
        public static bool SetValue(IFieldAccessor accessor, IMessage message, string value)
        {
            if (accessor.Descriptor.IsMap)
            {
                return false;
            }
            if (accessor.Descriptor.IsRepeated)
            {
                return false;
            }
            switch (accessor.Descriptor.FieldType)
            {
                case FieldType.Bool:
                    if (value == "1" || value.ToLower() == "true")
                    {
                        accessor.SetValue(message, true);
                    }
                    else
                    {
                        accessor.SetValue(message, false);
                    }
                    return true;

                case FieldType.Bytes:
                    return false;

                case FieldType.String:
                    accessor.SetValue(message, value);
                    return true;

                case FieldType.Double:
                    accessor.SetValue(message, double.Parse(value));
                    return true;

                case FieldType.SInt32:
                case FieldType.Int32:
                case FieldType.SFixed32:
                case FieldType.Enum:
                    accessor.SetValue(message, int.Parse(value));
                    return true;

                case FieldType.Fixed32:
                case FieldType.UInt32:
                    accessor.SetValue(message, uint.Parse(value));
                    return true;

                case FieldType.Fixed64:
                case FieldType.UInt64:
                    accessor.SetValue(message, ulong.Parse(value));
                    return true;

                case FieldType.SFixed64:
                case FieldType.Int64:
                case FieldType.SInt64:
                    accessor.SetValue(message, long.Parse(value));
                    return true;

                case FieldType.Float:
                    accessor.SetValue(message, float.Parse(value));
                    return true;

                default:
                    return false;
            }
        }
    }
}
