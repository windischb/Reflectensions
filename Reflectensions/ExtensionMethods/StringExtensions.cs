using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Reflectensions.Helpers;

namespace Reflectensions.ExtensionMethods
{
    public static class StringExtensions
    {
       
        public static string RemoveEmptyLines(this string value)
        {
            
            var val = Regex.Replace(value, @"^\s*$\n|\r", "", RegexOptions.Multiline);
            return val;
        }

        public static string Trim(this string value, params string[] trimCharacters) {
            return value.Trim(String.Join("", trimCharacters).ToCharArray());
        }

        public static string TrimToNull(this string input, params string[] trimCharacters) {
            return input.ToNull()?.Trim(String.Join("", trimCharacters).ToCharArray()).ToNull();
        }

        public static string[] Split(this string value, string split, bool removeEmptyEntries = false)
        {

            return value.Split(new[] { split }, removeEmptyEntries ? StringSplitOptions.RemoveEmptyEntries : StringSplitOptions.None);
        }


        #region StringIs
        public static bool IsNullOrWhiteSpace(this string value)
        {
            return String.IsNullOrWhiteSpace(value);
        }

        public static bool IsNumeric(this string s)
        {
            if (s.IsNullOrWhiteSpace())
                return false;

            bool firstchar = true;
            foreach (char c in s)
            {
                if (firstchar)
                {
                    if (!char.IsDigit(c) && c != '.' && c != '-' && c != '+')
                    {
                        return false;
                    }
                }
                else
                {
                    if (!char.IsDigit(c) && c != '.')
                    {
                        return false;
                    }
                }
                firstchar = false;

            }

            return true;
        }

        public static bool IsInt(this string str)
        {
            if (!IsNumeric(str))
                return false;

            int n;
            return int.TryParse(str, out n);

        }

        public static bool IsLong(this string str)
        {
            if (!IsNumeric(str))
                return false;

            long n;
            return long.TryParse(str, out n);

        }

        public static bool IsDouble(this string str)
        {
            if (!IsNumeric(str))
                return false;

            double n;
            return double.TryParse(str, out n);

        }

        public static bool IsDateTime(this string str, string customFormat = null) {

            return str.ToNullableDateTime() != null;

        }

        public static bool IsBoolean(this string str)
        {
            bool ret;
            return bool.TryParse(str, out ret);
        }

        public static bool IsValidIp(this string str)
        {
            System.Net.IPAddress ipAddress;
            return System.Net.IPAddress.TryParse(str, out ipAddress);
        }

       
        public static bool IsBase64Encoded(this string str)
        {
            if (string.IsNullOrEmpty(str))
                return false;

            try
            {
                byte[] data = Convert.FromBase64String(str);
                return (str.Replace(" ", "").Length % 4 == 0);
            }
            catch
            {
                return false;
            }
        }

        public static bool IsLowerCase(this string str)
        {
            return !string.IsNullOrEmpty(str) && !str.Any(char.IsUpper);
        }

        public static bool IsUpperCase(this string str)
        {
            return !string.IsNullOrEmpty(str) && !str.Any(char.IsLower);
        }

        private static string DomainMapper(Match match)
        {
            // IdnMapping class with default property values.
            var idn = new IdnMapping();

            string domainName = match.Groups[2].Value;
            domainName = idn.GetAscii(domainName);
            return match.Groups[1].Value + domainName;
        }

        public static Boolean IsValidEmailAddress(this string value)
        {
            //Boolean invalid = false;
            if (String.IsNullOrEmpty(value))
                return false;

            // Use IdnMapping class to convert Unicode domain names. 
            try
            {
                value = Regex.Replace(value, @"(@)(.+)$", DomainMapper);
            }
            catch
            {
                return false;
            }

            // Return true if strIn is in valid e-mail format. 
            try
            {
                return Regex.IsMatch(value,
                        @"^(?("")(""[^""]+?""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                        @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9]{2,17}))$",
                        RegexOptions.IgnoreCase);
            }
            catch
            {
                return false;
            }
        }


        public static bool IsGuid(this string value) {
            value = value.Trim("\"");

            return Guid.TryParse(value, out _);
        }

        #endregion

        #region StringTo
        public static String ToNull(this string value) {
            return String.IsNullOrEmpty(value) ? null : value;
        }

        public static int ToInt(this string str) {
            return !str.IsInt() ? default(int) : int.Parse(str);
        }
        public static int? ToNullableInt(this string value)
        {
            if (value == null)
                return null;

            if (int.TryParse(value, out var i))
            {
                return i;
            }
            return null;
        }

        public static decimal ToDecimal(this string str) {
            return !str.IsNumeric() ? default(decimal) : decimal.Parse(str);
        }
        public static decimal? ToNullableDecimal(this string value)
        {
            if (value == null)
                return null;

            if (decimal.TryParse(value, out var i))
            {
                return i;
            }
            return null;
        }

        public static float ToFloat(this string str) {
            return !str.IsInt() ? default(float) : float.Parse(str);
        }
        public static float? ToNullableFloat(this string value)
        {
            if (value == null)
                return null;

            if (float.TryParse(value, out var i))
            {
                return i;
            }
            return null;
        }

        public static long ToLong(this string str) {
            return !str.IsLong() ? default(long) : long.Parse(str);
        }
        public static long? ToNullableLong(this string value) {
            if (value == null)
                return null;

            if (long.TryParse(value, out var i)) {
                return i;
            }
            return null;
        }

        public static double ToDouble(this string str)
        {
            if (!str.IsDouble())
                return default(double);

            return double.Parse(str);

        }
        public static double? ToNullableDouble(this string value) {
            if (value == null)
                return null;

            if (double.TryParse(value, out var i)) {
                return i;
            }
            return null;
        }


        public static bool ToBoolean(this string str)
        {
            if (!str.IsBoolean())
                return default(bool);

            return bool.Parse(str);
        }

        public static DateTime? ToNullableDateTime(this string str, string customFormat = null)
        {

            if (String.IsNullOrWhiteSpace(str?.Trim('"')))
                return null;

            DateTime dateTime = default(DateTime);

            List<string> formats = new List<string>();

            if (!String.IsNullOrEmpty(customFormat))
            {
                formats.Add(customFormat);
            }
            else
            {
                formats = new List<string>{

                    "d.M.yyyy",
                    "dd.M.yyyy",
                    "d.MM.yyyy",
                    "dd.MM.yyyy",

                    "d.M.yyyy HH:mm",
                    "dd.M.yyyy HH:mm",
                    "d.MM.yyyy HH:mm",
                    "dd.MM.yyyy HH:mm",

                    "d.M.yyyy HH:mm:ss",
                    "d.MM.yyyy HH:mm:ss",
                    "dd.M.yyyy HH:mm:ss",
                    "dd.MM.yyyy HH:mm:ss",

                    "M/d/yyyy h:mm:ss tt",
                    "M/d/yyyy h:mm tt",
                    "MM/dd/yyyy hh:mm:ss",
                    "M/d/yyyy h:mm:ss",
                    "M/d/yyyy hh:mm tt",
                    "M/d/yyyy hh tt",
                    "M/d/yyyy h:mm",
                    "M/d/yyyy h:mm",
                    "MM/dd/yyyy hh:mm",
                    "M/dd/yyyy hh:mm"
                };
            }


            if (DateTime.TryParseExact(str, formats.ToArray(),
                CultureInfo.CurrentCulture,
                System.Globalization.DateTimeStyles.AssumeLocal, out dateTime))
            {
                return dateTime;
            }

            if (JsonHelpers.IsAvailable) {
                try {
                    string vstr = str;
                    if (!vstr.StartsWith("\"") && !vstr.EndsWith("\""))
                        vstr = $"\"{str}\"";

                    dateTime = JsonHelpers.ConvertTo<DateTime>(vstr);
                    return dateTime;
                } catch {
                    // ignored
                }
            }
            

            return null;
        }

        public static DateTime ToDateTime(this string str, string customFormat = null)
        {
            return ToNullableDateTime(str, customFormat) ?? default(DateTime);
        }

        public static string EncodeToBase64(this string toEncode)
        {
            if (string.IsNullOrEmpty(toEncode))
                return null;

            var toEncodeAsBytes = System.Text.Encoding.ASCII.GetBytes(toEncode);
            var returnValue = Convert.ToBase64String(toEncodeAsBytes);
            return returnValue;
        }

        public static string DecodeFromBase64(this string encodedData)
        {
            if (string.IsNullOrEmpty(encodedData))
                return null;

            var encodedDataAsBytes = Convert.FromBase64String(encodedData);
            var returnValue = System.Text.Encoding.ASCII.GetString(encodedDataAsBytes);
            return returnValue;
        }

        public static Guid ToGuid(this string value)
        {
            value = value.Trim("\"");

            if (Guid.TryParse(value, out var g)) {
                return g;
            }

            throw new InvalidCastException($"Can't cast '{value}' to GUID");
        }

        

        #endregion
    }
}
