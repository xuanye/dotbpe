using System;
using System.Security.Cryptography;
using System.Text;

namespace DotBPE.Utils.Utility
{
   /// <summary>
    /// </summary>
    public class CryptographyManager
    {
        private static readonly byte[] _slat =
        {
            83,
            110,
            100,
            97,
            32,
            67,
            82,
            77,
            32,
            88,
            117,
            97,
            110,
            121,
            101
        };

        public static string Md5Encrypt(string encryptingString)
        {
            var md5 = System.Security.Cryptography.MD5.Create();
            byte[] data = md5.ComputeHash(Encoding.UTF8.GetBytes(encryptingString));

            StringBuilder sBuilder = new StringBuilder();

            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            return sBuilder.ToString();
        }

        public static string AESEncrypt(string toEncrypt, string password)
        {
            var rfc2898DeriveBytes = new Rfc2898DeriveBytes(password, _slat, 1024);
            return AESEncrypt(toEncrypt, rfc2898DeriveBytes.GetBytes(32), rfc2898DeriveBytes.GetBytes(16));
        }

        public static string AESEncrypt(string toEncrypt, byte[] keyArray, byte[] ivArray)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(toEncrypt);
            var cryptoTransform = Aes.Create().CreateEncryptor(keyArray, ivArray);

            byte[] array = cryptoTransform.TransformFinalBlock(bytes, 0, bytes.Length);
            return Convert.ToBase64String(array, 0, array.Length);
        }

        public static Rfc2898DeriveBytes RFCDB(string password)
        {
            return new Rfc2898DeriveBytes(password, _slat, 1024);
        }

        public static string AESDecrypt(string toDecrypt, string password)
        {
            var rfc2898DeriveBytes = new Rfc2898DeriveBytes(password, _slat, 1024);
            return AESDecrypt(toDecrypt, rfc2898DeriveBytes.GetBytes(32), rfc2898DeriveBytes.GetBytes(16));
        }

        public static string AESDecrypt(string toDecrypt, byte[] keyArray, byte[] ivArray)
        {
            byte[] array = Convert.FromBase64String(toDecrypt);
            var cryptoTransform = Aes.Create().CreateEncryptor(keyArray, ivArray);

            byte[] bytes = cryptoTransform.TransformFinalBlock(array, 0, array.Length);
            string @string = Encoding.UTF8.GetString(bytes);
            return @string.Replace("\0", "");
        }
    }
}
