using System;
using System.Text;
using System.Text.RegularExpressions;

namespace Survey.Service
{
    public class Validator
    {
        #region 匹配方法

        /// <summary>
        /// 验证字符串是否匹配正则表达式描述的规则
        /// </summary>
        /// <param name="inputStr">待验证的字符串</param>
        /// <param name="patternStr">正则表达式字符串</param>
        /// <returns>是否匹配</returns>
        public static bool IsMatch(string inputStr, string patternStr)
        {
            return IsMatch(inputStr, patternStr, false, false);
        }

        /// <summary>
        /// 验证字符串是否匹配正则表达式描述的规则
        /// </summary>
        /// <param name="inputStr">待验证的字符串</param>
        /// <param name="patternStr">正则表达式字符串</param>
        /// <param name="ifIgnoreCase">匹配时是否不区分大小写</param>
        /// <returns>是否匹配</returns>
        public static bool IsMatch(string inputStr, string patternStr, bool ifIgnoreCase)
        {
            return IsMatch(inputStr, patternStr, ifIgnoreCase, false);
        }

        /// <summary>
        /// 验证字符串是否匹配正则表达式描述的规则
        /// </summary>
        /// <param name="inputStr">待验证的字符串</param>
        /// <param name="patternStr">正则表达式字符串</param>
        /// <param name="ifIgnoreCase">匹配时是否不区分大小写</param>
        /// <param name="ifValidateWhiteSpace">是否验证空白字符串</param>
        /// <returns>是否匹配</returns>
        public static bool IsMatch(string inputStr, string patternStr, bool ifIgnoreCase, bool ifValidateWhiteSpace)
        {
            if (!ifValidateWhiteSpace && string.IsNullOrWhiteSpace(inputStr))
                return false;//如果不要求验证空白字符串而此时传入的待验证字符串为空白字符串，则不匹配
            Regex regex = null;
            if (ifIgnoreCase)
                regex = new Regex(patternStr, RegexOptions.IgnoreCase);//指定不区分大小写的匹配
            else
                regex = new Regex(patternStr);
            return regex.IsMatch(inputStr);
        }

        #endregion 匹配方法

        #region 验证方法

        /// <summary>
        /// 验证数字(double类型)
        /// [可以包含负号和小数点]
        /// </summary>
        /// <param name="input">待验证的字符串</param>
        /// <returns>是否匹配</returns>
        public static bool IsNumber(string input)
        {
            //string pattern = @"^-?\d+$|^(-?\d+)(\.\d+)?$";
            //return IsMatch(input, pattern);
            double d = 0;
            if (double.TryParse(input, out d))
                return true;
            else
                return false;
        }

        /// <summary>
        /// 验证整数
        /// </summary>
        /// <param name="input">待验证的字符串</param>
        /// <returns>是否匹配</returns>
        public static bool IsInteger(string input)
        {
            //string pattern = @"^-?\d+$";
            //return IsMatch(input, pattern);
            int i = 0;
            if (int.TryParse(input, out i))
                return true;
            else
                return false;
        }

        /// <summary>
        /// 验证非负整数
        /// </summary>
        /// <param name="input">待验证的字符串</param>
        /// <returns>是否匹配</returns>
        public static bool IsIntegerNotNagtive(string input)
        {
            //string pattern = @"^\d+$";
            //return IsMatch(input, pattern);
            int i = -1;
            if (int.TryParse(input, out i) && i >= 0)
                return true;
            else
                return false;
        }

        /// <summary>
        /// 验证正整数
        /// </summary>
        /// <param name="input">待验证的字符串</param>
        /// <returns>是否匹配</returns>
        public static bool IsIntegerPositive(string input)
        {
            //string pattern = @"^[0-9]*[1-9][0-9]*$";
            //return IsMatch(input, pattern);
            int i = 0;
            if (int.TryParse(input, out i) && i >= 1)
                return true;
            else
                return false;
        }

        /// <summary>
        /// 验证小数
        /// </summary>
        /// <param name="input">待验证的字符串</param>
        /// <returns>是否匹配</returns>
        public static bool IsDecimal(string input)
        {
            string pattern = @"^([-+]?[1-9]\d*\.\d+|-?0\.\d*[1-9]\d*)$";
            return IsMatch(input, pattern);
        }

        /// <summary>
        /// 验证只包含英文字母
        /// </summary>
        /// <param name="input">待验证的字符串</param>
        /// <returns>是否匹配</returns>
        public static bool IsEnglishCharacter(string input)
        {
            string pattern = @"^[A-Za-z]+$";
            return IsMatch(input, pattern);
        }

        /// <summary>
        /// 验证只包含数字和英文字母
        /// </summary>
        /// <param name="input">待验证的字符串</param>
        /// <returns>是否匹配</returns>
        public static bool IsIntegerAndEnglishCharacter(string input)
        {
            string pattern = @"^[0-9A-Za-z]+$";
            return IsMatch(input, pattern);
        }

        /// <summary>
        /// 验证只包含汉字
        /// </summary>
        /// <param name="input">待验证的字符串</param>
        /// <returns>是否匹配</returns>
        public static bool IsChineseCharacter(string input)
        {
            string pattern = @"^[\u4e00-\u9fa5]+$";
            return IsMatch(input, pattern);
        }

        /// <summary>
        /// 验证数字长度范围（数字前端的0计长度）
        /// [若要验证固定长度，可传入相同的两个长度数值]
        /// </summary>
        /// <param name="input">待验证的字符串</param>
        /// <param name="lengthBegin">长度范围起始值（含）</param>
        /// <param name="lengthEnd">长度范围结束值（含）</param>
        /// <returns>是否匹配</returns>
        public static bool IsIntegerLength(string input, int lengthBegin, int lengthEnd)
        {
            //string pattern = @"^\d{" + lengthBegin + "," + lengthEnd + "}$";
            //return IsMatch(input, pattern);
            if (input.Length >= lengthBegin && input.Length <= lengthEnd)
            {
                int i;
                if (int.TryParse(input, out i))
                    return true;
                else
                    return false;
            }
            else
                return false;
        }

        /// <summary>
        /// 验证字符串包含内容
        /// </summary>
        /// <param name="input">待验证的字符串</param>
        /// <param name="withEnglishCharacter">是否包含英文字母</param>
        /// <param name="withNumber">是否包含数字</param>
        /// <param name="withChineseCharacter">是否包含汉字</param>
        /// <returns>是否匹配</returns>
        public static bool IsStringInclude(string input, bool withEnglishCharacter, bool withNumber, bool withChineseCharacter)
        {
            if (!withEnglishCharacter && !withNumber && !withChineseCharacter)
                return false;//如果英文字母、数字和汉字都没有，则返回false
            StringBuilder patternString = new StringBuilder();
            patternString.Append("^[");
            if (withEnglishCharacter)
                patternString.Append("a-zA-Z");
            if (withNumber)
                patternString.Append("0-9");
            if (withChineseCharacter)
                patternString.Append(@"\u4E00-\u9FA5");
            patternString.Append("]+$");
            return IsMatch(input, patternString.ToString());
        }

        /// <summary>
        /// 验证字符串长度范围
        /// [若要验证固定长度，可传入相同的两个长度数值]
        /// </summary>
        /// <param name="input">待验证的字符串</param>
        /// <param name="lengthBegin">长度范围起始值（含）</param>
        /// <param name="lengthEnd">长度范围结束值（含）</param>
        /// <returns>是否匹配</returns>
        public static bool IsStringLength(string input, int lengthBegin, int lengthEnd)
        {
            //string pattern = @"^.{" + lengthBegin + "," + lengthEnd + "}$";
            //return IsMatch(input, pattern);
            if (input.Length >= lengthBegin && input.Length <= lengthEnd)
                return true;
            else
                return false;
        }

        /// <summary>
        /// 验证字符串长度范围（字符串内只包含数字和/或英文字母）
        /// [若要验证固定长度，可传入相同的两个长度数值]
        /// </summary>
        /// <param name="input">待验证的字符串</param>
        /// <param name="lengthBegin">长度范围起始值（含）</param>
        /// <param name="lengthEnd">长度范围结束值（含）</param>
        /// <returns>是否匹配</returns>
        public static bool IsStringLengthOnlyNumberAndEnglishCharacter(string input, int lengthBegin, int lengthEnd)
        {
            string pattern = @"^[0-9a-zA-z]{" + lengthBegin + "," + lengthEnd + "}$";
            return IsMatch(input, pattern);
        }

        /// <summary>
        /// 验证字符串长度范围
        /// [若要验证固定长度，可传入相同的两个长度数值]
        /// </summary>
        /// <param name="input">待验证的字符串</param>
        /// <param name="withEnglishCharacter">是否包含英文字母</param>
        /// <param name="withNumber">是否包含数字</param>
        /// <param name="withChineseCharacter">是否包含汉字</param>
        /// <param name="lengthBegin">长度范围起始值（含）</param>
        /// <param name="lengthEnd">长度范围结束值（含）</param>
        /// <returns>是否匹配</returns>
        public static bool IsStringLengthByInclude(string input, bool withEnglishCharacter, bool withNumber, bool withChineseCharacter, int lengthBegin, int lengthEnd)
        {
            if (!withEnglishCharacter && !withNumber && !withChineseCharacter)
                return false;//如果英文字母、数字和汉字都没有，则返回false
            StringBuilder patternString = new StringBuilder();
            patternString.Append("^[");
            if (withEnglishCharacter)
                patternString.Append("a-zA-Z");
            if (withNumber)
                patternString.Append("0-9");
            if (withChineseCharacter)
                patternString.Append(@"\u4E00-\u9FA5");
            patternString.Append("]{" + lengthBegin + "," + lengthEnd + "}$");
            return IsMatch(input, patternString.ToString());
        }

        /// <summary>
        /// 验证字符串字节数长度范围
        /// [若要验证固定长度，可传入相同的两个长度数值；每个汉字为两个字节长度]
        /// </summary>
        /// <param name="input">待验证的字符串</param>
        /// <param name="lengthBegin">长度范围起始值（含）</param>
        /// <param name="lengthEnd">长度范围结束值（含）</param>
        /// <returns></returns>
        public static bool IsStringByteLength(string input, int lengthBegin, int lengthEnd)
        {
            //int byteLength = Regex.Replace(input, @"[^\x00-\xff]", "ok").Length;
            //if (byteLength >= lengthBegin && byteLength <= lengthEnd)
            //{
            //    return true;
            //}
            //return false;
            int byteLength = Encoding.Default.GetByteCount(input);
            if (byteLength >= lengthBegin && byteLength <= lengthEnd)
                return true;
            else
                return false;
        }

        /// <summary>
        /// 验证日期
        /// </summary>
        /// <param name="input">待验证的字符串</param>
        /// <returns>是否匹配</returns>
        public static bool IsDateTime(string input)
        {
            DateTime dt;
            if (DateTime.TryParse(input, out dt))
                return true;
            else
                return false;
        }

        /// <summary>
        /// 验证固定电话号码
        /// [3位或4位区号；区号可以用小括号括起来；区号可以省略；区号与本地号间可以用减号或空格隔开；可以有3位数的分机号，分机号前要加减号]
        /// </summary>
        /// <param name="input">待验证的字符串</param>
        /// <returns>是否匹配</returns>
        public static bool IsTelePhoneNumber(string input)
        {
            string pattern = @"^(((0\d2|0\d{2})[- ]?)?\d{8}|((0\d3|0\d{3})[- ]?)?\d{7})(-\d{3})?$";
            return IsMatch(input, pattern);
        }

        /// <summary>
        /// 验证手机号码
        /// [可匹配"(+86)013325656352"，括号可以省略，+号可以省略，(+86)可以省略，11位手机号前的0可以省略；11位手机号第二位数可以是3、4、5、8中的任意一个]
        /// </summary>
        /// <param name="input">待验证的字符串</param>
        /// <returns>是否匹配</returns>
        public static bool IsMobilePhoneNumber(string input)
        {
            string pattern = @"^((\+)?86|((\+)?86)?)0?1[3458]\d{9}$";
            return IsMatch(input, pattern);
        }

        /// <summary>
        /// 验证电话号码（可以是固定电话号码或手机号码）
        /// [固定电话：[3位或4位区号；区号可以用小括号括起来；区号可以省略；区号与本地号间可以用减号或空格隔开；可以有3位数的分机号，分机号前要加减号]]
        /// [手机号码：[可匹配"(+86)013325656352"，括号可以省略，+号可以省略，(+86)可以省略，手机号前的0可以省略；手机号第二位数可以是3、4、5、8中的任意一个]]
        /// </summary>
        /// <param name="input">待验证的字符串</param>
        /// <returns>是否匹配</returns>
        public static bool IsPhoneNumber(string input)
        {
            string pattern = @"^((\+)?86|((\+)?86)?)0?1[3458]\d{9}$|^(((0\d2|0\d{2})[- ]?)?\d{8}|((0\d3|0\d{3})[- ]?)?\d{7})(-\d{3})?$";
            return IsMatch(input, pattern);
        }

        /// <summary>
        /// 验证邮政编码
        /// </summary>
        /// <param name="input">待验证的字符串</param>
        /// <returns>是否匹配</returns>
        public static bool IsZipCode(string input)
        {
            //string pattern = @"^\d{6}$";
            //return IsMatch(input, pattern);
            if (input.Length != 6)
                return false;
            int i;
            if (int.TryParse(input, out i))
                return true;
            else
                return false;
        }

        /// <summary>
        /// 验证电子邮箱
        /// [@字符前可以包含字母、数字、下划线和点号；@字符后可以包含字母、数字、下划线和点号；@字符后至少包含一个点号且点号不能是最后一个字符；最后一个点号后只能是字母或数字]
        /// </summary>
        /// <param name="input">待验证的字符串</param>
        /// <returns>是否匹配</returns>
        public static bool IsEmail(string input)
        {
            ////邮箱名以数字或字母开头；邮箱名可由字母、数字、点号、减号、下划线组成；邮箱名（@前的字符）长度为3～18个字符；邮箱名不能以点号、减号或下划线结尾；不能出现连续两个或两个以上的点号、减号。
            //string pattern = @"^[a-zA-Z0-9]((?<!(\.\.|--))[a-zA-Z0-9\._-]){1,16}[a-zA-Z0-9]@([0-9a-zA-Z][0-9a-zA-Z-]{0,62}\.)+([0-9a-zA-Z][0-9a-zA-Z-]{0,62})\.?|((25[0-5]|2[0-4]\d|[01]?\d\d?)\.){3}(25[0-5]|2[0-4]\d|[01]?\d\d?)$";
            string pattern = @"^([\w-\.]+)@([\w-\.]+)(\.[a-zA-Z0-9]+)$";
            return IsMatch(input, pattern);
        }

        /// <summary>
        /// 验证网址（可以匹配IPv4地址但没对IPv4地址进行格式验证；IPv6暂时没做匹配）
        /// [允许省略"://"；可以添加端口号；允许层级；允许传参；域名中至少一个点号且此点号前要有内容]
        /// </summary>
        /// <param name="input">待验证的字符串</param>
        /// <returns>是否匹配</returns>
        public static bool IsURL(string input)
        {
            ////每级域名由字母、数字和减号构成（第一个字母不能是减号），不区分大小写，单个域长度不超过63，完整的域名全长不超过256个字符。在DNS系统中，全名是以一个点“.”来结束的，例如“www.nit.edu.cn.”。没有最后的那个点则表示一个相对地址。
            ////没有例如"http://"的前缀，没有传参的匹配
            //string pattern = @"^([0-9a-zA-Z][0-9a-zA-Z-]{0,62}\.)+([0-9a-zA-Z][0-9a-zA-Z-]{0,62})\.?$";

            //string pattern = @"^(((file|gopher|news|nntp|telnet|http|ftp|https|ftps|sftp)://)|(www\.))+(([a-zA-Z0-9\._-]+\.[a-zA-Z]{2,6})|([0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}))(/[a-zA-Z0-9\&%_\./-~-]*)?$";
            string pattern = @"^([a-zA-Z]+://)?([\w-\.]+)(\.[a-zA-Z0-9]+)(:\d{0,5})?/?([\w-/]*)\.?([a-zA-Z]*)\??(([\w-]*=[\w%]*&?)*)$";
            return IsMatch(input, pattern);
        }

        /// <summary>
        /// 验证IPv4地址
        /// [第一位和最后一位数字不能是0或255；允许用0补位]
        /// </summary>
        /// <param name="input">待验证的字符串</param>
        /// <returns>是否匹配</returns>
        public static bool IsIPv4(string input)
        {
            //string pattern = @"^(25[0-4]|2[0-4]\d]|[01]?\d{2}|[1-9])\.(25[0-5]|2[0-4]\d]|[01]?\d?\d)\.(25[0-5]|2[0-4]\d]|[01]?\d?\d)\.(25[0-4]|2[0-4]\d]|[01]?\d{2}|[1-9])$";
            //return IsMatch(input, pattern);
            string[] IPs = input.Split('.');
            if (IPs.Length != 4)
                return false;
            int n = -1;
            for (int i = 0; i < IPs.Length; i++)
            {
                if (i == 0 || i == 3)
                {
                    if (int.TryParse(IPs[i], out n) && n > 0 && n < 255)
                        continue;
                    else
                        return false;
                }
                else
                {
                    if (int.TryParse(IPs[i], out n) && n >= 0 && n <= 255)
                        continue;
                    else
                        return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 验证IPv6地址
        /// [可用于匹配任何一个合法的IPv6地址]
        /// </summary>
        /// <param name="input">待验证的字符串</param>
        /// <returns>是否匹配</returns>
        public static bool IsIPv6(string input)
        {
            string pattern = @"^\s*((([0-9A-Fa-f]{1,4}:){7}([0-9A-Fa-f]{1,4}|:))|(([0-9A-Fa-f]{1,4}:){6}(:[0-9A-Fa-f]{1,4}|((25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)(\.(25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)){3})|:))|(([0-9A-Fa-f]{1,4}:){5}(((:[0-9A-Fa-f]{1,4}){1,2})|:((25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)(\.(25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)){3})|:))|(([0-9A-Fa-f]{1,4}:){4}(((:[0-9A-Fa-f]{1,4}){1,3})|((:[0-9A-Fa-f]{1,4})?:((25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)(\.(25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)){3}))|:))|(([0-9A-Fa-f]{1,4}:){3}(((:[0-9A-Fa-f]{1,4}){1,4})|((:[0-9A-Fa-f]{1,4}){0,2}:((25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)(\.(25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)){3}))|:))|(([0-9A-Fa-f]{1,4}:){2}(((:[0-9A-Fa-f]{1,4}){1,5})|((:[0-9A-Fa-f]{1,4}){0,3}:((25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)(\.(25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)){3}))|:))|(([0-9A-Fa-f]{1,4}:){1}(((:[0-9A-Fa-f]{1,4}){1,6})|((:[0-9A-Fa-f]{1,4}){0,4}:((25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)(\.(25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)){3}))|:))|(:(((:[0-9A-Fa-f]{1,4}){1,7})|((:[0-9A-Fa-f]{1,4}){0,5}:((25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)(\.(25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)){3}))|:)))(%.+)?\s*$";
            return IsMatch(input, pattern);
        }

        /// <summary>
        /// 身份证上数字对应的地址
        /// </summary>
        //enum IDAddress
        //{
        //    北京 = 11, 天津 = 12, 河北 = 13, 山西 = 14, 内蒙古 = 15, 辽宁 = 21, 吉林 = 22, 黑龙江 = 23, 上海 = 31, 江苏 = 32, 浙江 = 33,
        //    安徽 = 34, 福建 = 35, 江西 = 36, 山东 = 37, 河南 = 41, 湖北 = 42, 湖南 = 43, 广东 = 44, 广西 = 45, 海南 = 46, 重庆 = 50, 四川 = 51,
        //    贵州 = 52, 云南 = 53, 西藏 = 54, 陕西 = 61, 甘肃 = 62, 青海 = 63, 宁夏 = 64, 新疆 = 65, 台湾 = 71, 香港 = 81, 澳门 = 82, 国外 = 91
        //}

        /// <summary>
        /// 验证一代身份证号（15位数）
        /// [长度为15位的数字；匹配对应省份地址；生日能正确匹配]
        /// </summary>
        /// <param name="input">待验证的字符串</param>
        /// <returns>是否匹配</returns>
        public static bool IsIDCard15(string input)
        {
            //验证是否可以转换为15位整数
            long l = 0;
            if (!long.TryParse(input, out l) || l.ToString().Length != 15)
            {
                return false;
            }
            //验证省份是否匹配
            //1~6位为地区代码，其中1、2位数为各省级政府的代码，3、4位数为地、市级政府的代码，5、6位数为县、区级政府代码。
            string address = "11,12,13,14,15,21,22,23,31,32,33,34,35,36,37,41,42,43,44,45,46,50,51,52,53,54,61,62,63,64,65,71,81,82,91,";
            if (!address.Contains(input.Remove(2) + ","))
            {
                return false;
            }
            //验证生日是否匹配
            string birthdate = input.Substring(6, 6).Insert(4, "/").Insert(2, "/");
            DateTime dt;
            if (!DateTime.TryParse(birthdate, out dt))
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 验证二代身份证号（18位数，GB11643-1999标准）
        /// [长度为18位；前17位为数字，最后一位(校验码)可以为大小写x；匹配对应省份地址；生日能正确匹配；校验码能正确匹配]
        /// </summary>
        /// <param name="input">待验证的字符串</param>
        /// <returns>是否匹配</returns>
        public static bool IsIDCard18(string input)
        {
            //验证是否可以转换为正确的整数
            long l = 0;
            if (!long.TryParse(input.Remove(17), out l) || l.ToString().Length != 17 || !long.TryParse(input.Replace('x', '0').Replace('X', '0'), out l))
            {
                return false;
            }
            //验证省份是否匹配
            //1~6位为地区代码，其中1、2位数为各省级政府的代码，3、4位数为地、市级政府的代码，5、6位数为县、区级政府代码。
            string address = "11,12,13,14,15,21,22,23,31,32,33,34,35,36,37,41,42,43,44,45,46,50,51,52,53,54,61,62,63,64,65,71,81,82,91,";
            if (!address.Contains(input.Remove(2) + ","))
            {
                return false;
            }
            //验证生日是否匹配
            string birthdate = input.Substring(6, 8).Insert(6, "/").Insert(4, "/");
            DateTime dt;
            if (!DateTime.TryParse(birthdate, out dt))
            {
                return false;
            }
            //校验码验证
            //校验码：
            //（1）十七位数字本体码加权求和公式
            //S = Sum(Ai * Wi), i = 0, ... , 16 ，先对前17位数字的权求和
            //Ai:表示第i位置上的身份证号码数字值
            //Wi:表示第i位置上的加权因子
            //Wi: 7 9 10 5 8 4 2 1 6 3 7 9 10 5 8 4 2
            //（2）计算模
            //Y = mod(S, 11)
            //（3）通过模得到对应的校验码
            //Y: 0 1 2 3 4 5 6 7 8 9 10
            //校验码: 1 0 X 9 8 7 6 5 4 3 2
            string[] arrVarifyCode = ("1,0,x,9,8,7,6,5,4,3,2").Split(',');
            string[] Wi = ("7,9,10,5,8,4,2,1,6,3,7,9,10,5,8,4,2").Split(',');
            char[] Ai = input.Remove(17).ToCharArray();
            int sum = 0;
            for (int i = 0; i < 17; i++)
            {
                sum += int.Parse(Wi[i]) * int.Parse(Ai[i].ToString());
            }
            int y = -1;
            Math.DivRem(sum, 11, out y);
            if (arrVarifyCode[y] != input.Substring(17, 1).ToLower())
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 验证身份证号（不区分一二代身份证号）
        /// </summary>
        /// <param name="input">待验证的字符串</param>
        /// <returns>是否匹配</returns>
        public static bool IsIDCard(string input)
        {
            if (input.Length == 18)
                return IsIDCard18(input);
            else if (input.Length == 15)
                return IsIDCard15(input);
            else
                return false;
        }

        /// <summary>
        /// 验证经度
        /// </summary>
        /// <param name="input">待验证的字符串</param>
        /// <returns>是否匹配</returns>
        public static bool IsLongitude(string input)
        {
            ////范围为-180～180，小数位数必须是1到5位
            //string pattern = @"^[-\+]?((1[0-7]\d{1}|0?\d{1,2})\.\d{1,5}|180\.0{1,5})$";
            //return IsMatch(input, pattern);
            float lon;
            if (float.TryParse(input, out lon) && lon >= -180 && lon <= 180)
                return true;
            else
                return false;
        }

        /// <summary>
        /// 验证纬度
        /// </summary>
        /// <param name="input">待验证的字符串</param>
        /// <returns>是否匹配</returns>
        public static bool IsLatitude(string input)
        {
            ////范围为-90～90，小数位数必须是1到5位
            //string pattern = @"^[-\+]?([0-8]?\d{1}\.\d{1,5}|90\.0{1,5})$";
            //return IsMatch(input, pattern);
            float lat;
            if (float.TryParse(input, out lat) && lat >= -90 && lat <= 90)
                return true;
            else
                return false;
        }

        /// <summary>
        /// 是否匹配账号规则 (字母开头，允许5-16字节，允许字母数字下划线)
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public static bool IsValidAccount(string account)
        {
            return IsMatch(account, @"^[a-zA-Z][a-zA-Z0-9_]{4,15}$");
        }

        /// <summary>
        /// 密码
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public static bool IsValidPassword(string password)
        {
            return IsMatch(password, @"^.{6,18}$");
        }

        #endregion 验证方法
    }
}
