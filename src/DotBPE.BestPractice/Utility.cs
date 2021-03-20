using System;
using System.Text;

namespace DotBPE.BestPractice
{
    public static class Utility
    {
        public static string ClearSQLInject(string input)
        {
            return !string.IsNullOrEmpty(input) ? input.Replace("--", "").Replace("'", "").Replace(";", "；") : "";
        }

        public static string Base64EnCode(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return "";
            }
            byte[] b = Encoding.UTF8.GetBytes(input);
            return Convert.ToBase64String(b);
        }

        public static string Base64Decode(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return "";
            }
            byte[] b = Convert.FromBase64String(input);
            return Encoding.UTF8.GetString(b);
        }
    }
}
