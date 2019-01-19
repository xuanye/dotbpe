using DotBPE.Baseline.Extensions;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;
using System.Threading;

namespace DotBPE.Baseline.Utility
{
    [Serializable]
    public struct ObjectId : IComparable<ObjectId>, IEquatable<ObjectId>, IConvertible
    {
        private static readonly ObjectId __emptyInstance = default(ObjectId);
        private static readonly int __staticMachine;
        private static readonly short __staticPid;
        private static int __staticIncrement;
        private static DateTime __unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        private readonly int _timestamp;
        private readonly int _machine;
        private readonly short _pid;
        private readonly int _increment;

        static ObjectId()
        {
            __staticMachine = (GetMachineHash() + AppDomain.CurrentDomain.Id) & 0x00ffffff;
            __staticIncrement = (new Random()).Next();

            try
            {
                __staticPid = (short)GetCurrentProcessId();
            }
            catch (SecurityException)
            {
                __staticPid = 0;
            }
        }

        public ObjectId(byte[] bytes)
        {
            if (bytes == null)
            {
                throw new ArgumentNullException("bytes");
            }
            Unpack(bytes, out this._timestamp, out this._machine, out this._pid, out this._increment);
        }

        internal ObjectId(byte[] bytes, int index)
        {
            this._timestamp = (bytes[index] << 24) | (bytes[index + 1] << 16) | (bytes[index + 2] << 8) | bytes[index + 3];
            this._machine = (bytes[index + 4] << 16) | (bytes[index + 5] << 8) | bytes[index + 6];
            this._pid = (short)((bytes[index + 7] << 8) | bytes[index + 8]);
            this._increment = (bytes[index + 9] << 16) | (bytes[index + 10] << 8) | bytes[index + 11];
        }

        public ObjectId(DateTime timestamp, int machine, short pid, int increment)
            : this(GetTimestampFromDateTime(timestamp), machine, pid, increment)
        {
        }

        public ObjectId(int timestamp, int machine, short pid, int increment)
        {
            if ((machine & 0xff000000) != 0)
            {
                throw new ArgumentOutOfRangeException("machine", "The machine value must be between 0 and 16777215 (it must fit in 3 bytes).");
            }
            if ((increment & 0xff000000) != 0)
            {
                throw new ArgumentOutOfRangeException("increment", "The increment value must be between 0 and 16777215 (it must fit in 3 bytes).");
            }

            this._timestamp = timestamp;
            this._machine = machine;
            this._pid = pid;
            this._increment = increment;
        }

        public ObjectId(string value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            Unpack(Utils.ParseHexString(value), out this._timestamp, out this._machine, out this._pid, out this._increment);
        }

        public static ObjectId Empty
        {
            get { return __emptyInstance; }
        }

        public int Timestamp
        {
            get { return this._timestamp; }
        }

        public int Machine
        {
            get { return this._machine; }
        }

        public short Pid
        {
            get { return this._pid; }
        }

        public int Increment
        {
            get { return this._increment; }
        }

        public DateTime CreationTime
        {
            get { return __unixEpoch.AddSeconds(this._timestamp); }
        }

        public static bool operator <(ObjectId lhs, ObjectId rhs)
        {
            return lhs.CompareTo(rhs) < 0;
        }

        public static bool operator <=(ObjectId lhs, ObjectId rhs)
        {
            return lhs.CompareTo(rhs) <= 0;
        }

        public static bool operator ==(ObjectId lhs, ObjectId rhs)
        {
            return lhs.Equals(rhs);
        }

        public static bool operator !=(ObjectId lhs, ObjectId rhs)
        {
            return !(lhs == rhs);
        }

        public static bool operator >=(ObjectId lhs, ObjectId rhs)
        {
            return lhs.CompareTo(rhs) >= 0;
        }

        public static bool operator >(ObjectId lhs, ObjectId rhs)
        {
            return lhs.CompareTo(rhs) > 0;
        }

        public static ObjectId GenerateNewId()
        {
            return GenerateNewId(GetTimestampFromDateTime(DateTime.UtcNow));
        }

        public static ObjectId GenerateNewId(DateTime timestamp)
        {
            return GenerateNewId(GetTimestampFromDateTime(timestamp));
        }

        public static ObjectId GenerateNewId(int timestamp)
        {
            int increment = Interlocked.Increment(ref __staticIncrement) & 0x00ffffff; // only use low order 3 bytes
            return new ObjectId(timestamp, __staticMachine, __staticPid, increment);
        }

        public static byte[] Pack(int timestamp, int machine, short pid, int increment)
        {
            if ((machine & 0xff000000) != 0)
            {
                throw new ArgumentOutOfRangeException("machine", "The machine value must be between 0 and 16777215 (it must fit in 3 bytes).");
            }
            if ((increment & 0xff000000) != 0)
            {
                throw new ArgumentOutOfRangeException("increment", "The increment value must be between 0 and 16777215 (it must fit in 3 bytes).");
            }

            byte[] bytes = new byte[12];
            bytes[0] = (byte)(timestamp >> 24);
            bytes[1] = (byte)(timestamp >> 16);
            bytes[2] = (byte)(timestamp >> 8);
            bytes[3] = (byte)(timestamp);
            bytes[4] = (byte)(machine >> 16);
            bytes[5] = (byte)(machine >> 8);
            bytes[6] = (byte)(machine);
            bytes[7] = (byte)(pid >> 8);
            bytes[8] = (byte)(pid);
            bytes[9] = (byte)(increment >> 16);
            bytes[10] = (byte)(increment >> 8);
            bytes[11] = (byte)(increment);
            return bytes;
        }

        public static ObjectId Parse(string s)
        {
            if (s == null)
            {
                throw new ArgumentNullException("s");
            }
            ObjectId objectId;
            if (TryParse(s, out objectId))
            {
                return objectId;
            }
            else
            {
                var message = string.Format("'{0}' is not a valid 24 digit hex string.", s);
                throw new FormatException(message);
            }
        }

        public static bool TryParse(string s, out ObjectId objectId)
        {
            if (s != null && s.Length == 24)
            {
                byte[] bytes;
                if (Utils.TryParseHexString(s, out bytes))
                {
                    objectId = new ObjectId(bytes);
                    return true;
                }
            }

            objectId = default(ObjectId);
            return false;
        }

        public static bool IsValid(string s)
        {
            ObjectId objectId;
            return TryParse(s, out objectId);
        }

        public static void Unpack(byte[] bytes, out int timestamp, out int machine, out short pid, out int increment)
        {
            if (bytes == null)
            {
                throw new ArgumentNullException("bytes");
            }
            if (bytes.Length != 12)
            {
                throw new ArgumentOutOfRangeException("bytes", "Byte array must be 12 bytes long.");
            }
            timestamp = (bytes[0] << 24) + (bytes[1] << 16) + (bytes[2] << 8) + bytes[3];
            machine = (bytes[4] << 16) + (bytes[5] << 8) + bytes[6];
            pid = (short)((bytes[7] << 8) + bytes[8]);
            increment = (bytes[9] << 16) + (bytes[10] << 8) + bytes[11];
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static int GetCurrentProcessId()
        {
            return Process.GetCurrentProcess().Id;
        }

        private static int GetMachineHash()
        {
            var hostName = Environment.MachineName;
            return 0x00ffffff & hostName.GetHashCode();
        }

        private static int GetTimestampFromDateTime(DateTime timestamp)
        {
            var secondsSinceEpoch = (long)Math.Floor((Utils.ToUniversalTime(timestamp) - __unixEpoch).TotalSeconds);
            if (secondsSinceEpoch < int.MinValue || secondsSinceEpoch > int.MaxValue)
            {
                throw new ArgumentOutOfRangeException("timestamp");
            }
            return (int)secondsSinceEpoch;
        }

        public int CompareTo(ObjectId other)
        {
            int r = this._timestamp.CompareTo(other._timestamp);
            if (r != 0) { return r; }
            r = this._machine.CompareTo(other._machine);
            if (r != 0) { return r; }
            r = this._pid.CompareTo(other._pid);
            if (r != 0) { return r; }
            return this._increment.CompareTo(other._increment);
        }

        public bool Equals(ObjectId rhs)
        {
            return this._timestamp == rhs._timestamp && this._machine == rhs._machine && this._pid == rhs._pid && this._increment == rhs._increment;
        }

        public override bool Equals(object obj)
        {
            if (obj is ObjectId)
            {
                return Equals((ObjectId)obj);
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            int hash = 17;
            hash = 37 * hash + this._timestamp.GetHashCode();
            hash = 37 * hash + this._machine.GetHashCode();
            hash = 37 * hash + this._pid.GetHashCode();
            hash = 37 * hash + this._increment.GetHashCode();
            return hash;
        }

        public byte[] ToByteArray()
        {
            return Pack(this._timestamp, this._machine, this._pid, this._increment);
        }

        public override string ToString()
        {
            return Pack(this._timestamp, this._machine, this._pid, this._increment).ToHex();
        }

        internal void GetBytes(byte[] bytes, int index)
        {
            bytes[index] = (byte)(this._timestamp >> 24);
            bytes[1 + index] = (byte)(this._timestamp >> 16);
            bytes[2 + index] = (byte)(this._timestamp >> 8);
            bytes[3 + index] = (byte)(this._timestamp);
            bytes[4 + index] = (byte)(this._machine >> 16);
            bytes[5 + index] = (byte)(this._machine >> 8);
            bytes[6 + index] = (byte)(this._machine);
            bytes[7 + index] = (byte)(this._pid >> 8);
            bytes[8 + index] = (byte)(this._pid);
            bytes[9 + index] = (byte)(this._increment >> 16);
            bytes[10 + index] = (byte)(this._increment >> 8);
            bytes[11 + index] = (byte)(this._increment);
        }

        TypeCode IConvertible.GetTypeCode()
        {
            return TypeCode.Object;
        }

        bool IConvertible.ToBoolean(IFormatProvider provider)
        {
            throw new InvalidCastException();
        }

        byte IConvertible.ToByte(IFormatProvider provider)
        {
            throw new InvalidCastException();
        }

        char IConvertible.ToChar(IFormatProvider provider)
        {
            throw new InvalidCastException();
        }

        DateTime IConvertible.ToDateTime(IFormatProvider provider)
        {
            throw new InvalidCastException();
        }

        decimal IConvertible.ToDecimal(IFormatProvider provider)
        {
            throw new InvalidCastException();
        }

        double IConvertible.ToDouble(IFormatProvider provider)
        {
            throw new InvalidCastException();
        }

        short IConvertible.ToInt16(IFormatProvider provider)
        {
            throw new InvalidCastException();
        }

        int IConvertible.ToInt32(IFormatProvider provider)
        {
            throw new InvalidCastException();
        }

        long IConvertible.ToInt64(IFormatProvider provider)
        {
            throw new InvalidCastException();
        }

        sbyte IConvertible.ToSByte(IFormatProvider provider)
        {
            throw new InvalidCastException();
        }

        float IConvertible.ToSingle(IFormatProvider provider)
        {
            throw new InvalidCastException();
        }

        string IConvertible.ToString(IFormatProvider provider)
        {
            return ToString();
        }

        object IConvertible.ToType(Type conversionType, IFormatProvider provider)
        {
            switch (Type.GetTypeCode(conversionType))
            {
                case TypeCode.String:
                    return ((IConvertible)this).ToString(provider);

                case TypeCode.Object:
                    if (conversionType == typeof(object) || conversionType == typeof(ObjectId))
                    {
                        return this;
                    }
                    break;
            }

            throw new InvalidCastException();
        }

        ushort IConvertible.ToUInt16(IFormatProvider provider)
        {
            throw new InvalidCastException();
        }

        uint IConvertible.ToUInt32(IFormatProvider provider)
        {
            throw new InvalidCastException();
        }

        ulong IConvertible.ToUInt64(IFormatProvider provider)
        {
            throw new InvalidCastException();
        }

        internal static class Utils
        {
            public static byte[] ParseHexString(string s)
            {
                if (s == null)
                {
                    throw new ArgumentNullException("s");
                }

                byte[] bytes;
                if ((s.Length & 1) != 0)
                {
                    s = "0" + s; // make length of s even
                }
                bytes = new byte[s.Length / 2];
                for (int i = 0; i < bytes.Length; i++)
                {
                    string hex = s.Substring(2 * i, 2);
                    try
                    {
                        byte b = Convert.ToByte(hex, 16);
                        bytes[i] = b;
                    }
                    catch (FormatException e)
                    {
                        throw new FormatException(
                            string.Format("Invalid hex string {0}. Problem with substring {1} starting at position {2}",
                            s,
                            hex,
                            2 * i),
                            e);
                    }
                }

                return bytes;
            }

            public static string ToHexString(byte[] bytes)
            {
                if (bytes == null)
                {
                    throw new ArgumentNullException("bytes");
                }
                var sb = new StringBuilder(bytes.Length * 2);
                foreach (var b in bytes)
                {
                    sb.AppendFormat("{0:x2}", b);
                }
                return sb.ToString();
            }

            public static DateTime ToUniversalTime(DateTime dateTime)
            {
                if (dateTime == DateTime.MinValue)
                {
                    return DateTime.SpecifyKind(DateTime.MinValue, DateTimeKind.Utc);
                }
                else if (dateTime == DateTime.MaxValue)
                {
                    return DateTime.SpecifyKind(DateTime.MaxValue, DateTimeKind.Utc);
                }
                else
                {
                    return dateTime.ToUniversalTime();
                }
            }

            public static bool TryParseHexString(string s, out byte[] bytes)
            {
                try
                {
                    bytes = ParseHexString(s);
                }
                catch
                {
                    bytes = null;
                    return false;
                }

                return true;
            }
        }
    }
}
