using Google.Protobuf;
using Google.Protobuf.Reflection;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotBPE.BestPractice
{
    public static class ProtobufExtensions
    {
        /// <summary>
        /// 判断是否登录过
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static bool IsAuthenticated(this IMessage message)
        {
            var identityField = message.Descriptor.FindFieldByNumber(2);
            if (identityField == null)
            {
                return false;
            }
            object o = identityField.Accessor.GetValue(message);

            return o != null && !string.IsNullOrEmpty(o.ToString());
        }

        public static bool SetValue(this IMessage message, int num, string value)
        {
            var field = message.Descriptor.FindFieldByNumber(num);
            if (field == null)
            {
                return false;
            }
            return SetValue(field.Accessor, message, value);
        }

        public static void PrepareCommonParams(this IMessage my, IMessage from)
        {
            PrepareField(my, from, 1);
            PrepareField(my, from, 2);
            PrepareField(my, from, 3);
            PrepareField(my, from, 4);
            PrepareField(my, from, 5);
            PrepareField(my, from, 6);
            PrepareField(my, from, 7);
            PrepareField(my, from, 8);
        }

        private static void PrepareField(IMessage target, IMessage from, int filedNum)
        {
            var fromFiled = from.Descriptor.FindFieldByNumber(filedNum);
            var targetFiled = target.Descriptor.FindFieldByNumber(filedNum);

            if (fromFiled == null || targetFiled == null)
            {
                return;
            }

            if (fromFiled.FieldType == targetFiled.FieldType)
            {
                var fromValue = fromFiled.Accessor.GetValue(from);
                if (fromValue != null)
                {
                    targetFiled.Accessor.SetValue(target, fromValue);
                }
            }
        }

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
                    if (value == "1" || string.Equals(value, "true", StringComparison.OrdinalIgnoreCase))
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
                    if (double.TryParse(value, out var doubleValue))
                    {
                        accessor.SetValue(message, doubleValue);
                        return true;
                    }
                    return false;

                case FieldType.SInt32:
                case FieldType.Int32:
                case FieldType.SFixed32:
                case FieldType.Enum:
                    if (int.TryParse(value, out var intValue))
                    {
                        accessor.SetValue(message, intValue);
                        return true;
                    }
                    return false;

                case FieldType.Fixed32:
                case FieldType.UInt32:
                    if (uint.TryParse(value, out var untValue))
                    {
                        accessor.SetValue(message, untValue);
                        return true;
                    }
                    return false;

                case FieldType.Fixed64:
                case FieldType.UInt64:
                    if (ulong.TryParse(value, out var ulongValue))
                    {
                        accessor.SetValue(message, ulongValue);
                        return true;
                    }
                    return false;

                case FieldType.SFixed64:
                case FieldType.Int64:
                case FieldType.SInt64:
                    if (long.TryParse(value, out var longValue))
                    {
                        accessor.SetValue(message, longValue);
                        return true;
                    }
                    return false;

                case FieldType.Float:
                    if (float.TryParse(value, out var floatValue))
                    {
                        accessor.SetValue(message, floatValue);
                        return true;
                    }
                    return false;

                default:
                    return false;
            }
        }
    }
}
