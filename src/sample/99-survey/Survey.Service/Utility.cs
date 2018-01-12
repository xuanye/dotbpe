using System;
using System.Text;

namespace Survey.Service
{
    /// <summary>
    /// 公共的帮助类
    /// </summary>
    public static class Utility
    {
        public static string ClearSafeStringParma(string input)
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

        /// <summary>
        /// 计算所在周的周一的日期
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static DateTime GetMondayDateTime(DateTime date)
        {
            int span = date.DayOfWeek.GetHashCode() == 0 ? 7 : date.DayOfWeek.GetHashCode();
            return new DateTime(date.Year, date.Month, date.Day).AddDays(1 - span);
        }

        /// <summary>
        /// 计算所在周的周六的日期
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static DateTime GetSaturdayDateTime(DateTime date)
        {
            int span = date.DayOfWeek.GetHashCode() == 0 ? 7 : date.DayOfWeek.GetHashCode();
            return new DateTime(date.Year, date.Month, date.Day).AddDays(6 - span);
        }

        public static string ConvertYM(string ym)
        {
            return ConvertYM(ym, string.Empty);
        }

        /// <summary>
        /// 转换年月字符串   201512->2015年XX12月
        /// </summary>
        /// <param name="ym"></param>
        /// <returns></returns>
        public static string ConvertYM(string ym, string separator)
        {
            if (string.IsNullOrEmpty(ym) || ym.Length < 5)
            {
                throw new Exception("转换年月字符串出错");
            }
            int year = Convert.ToInt32(ym.Substring(0, 4));
            int month = Convert.ToInt32(ym.Substring(4, 2));

            if (separator == null)
                separator = string.Empty;
            return string.Format("{0}年{1}{2}月份", year, separator, month);
        }
    }
}
