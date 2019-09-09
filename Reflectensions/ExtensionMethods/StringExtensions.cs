using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Reflectensions.Helper;
using Reflectensions.HelperClasses;

namespace Reflectensions.ExtensionMethods {
    public static class StringExtensions {

        public static string[] Split(this string value, string split, StringSplitOptions options) => StringHelpers.Split(value, split, options);

        public static string[] Split(this string value, string split, bool removeEmptyEntries = false) => StringHelpers.Split(value, split, removeEmptyEntries);


        public static StringDifferences Differences(this string value, string pattern, bool ignoreCase = true) => StringHelpers.Differences(value, pattern, ignoreCase);

        public static int DifferencesCount(this string value, string pattern) => StringHelpers.DifferencesCount(value, pattern);


        public static string Trim(this string value, params string[] trimCharacters) => StringHelpers.Trim(value, trimCharacters);

        #region StringIs
        public static bool IsNullOrWhiteSpace(this string value) => StringHelpers.IsNullOrWhiteSpace(value);

        public static bool IsNumeric(this string value) => StringHelpers.IsNumeric(value);

        public static bool IsInt(this string value) => StringHelpers.IsInt(value);

        public static bool IsLong(this string value) => StringHelpers.IsLong(value);

        public static bool IsDouble(this string value) => StringHelpers.IsDouble(value);

        public static bool IsDateTime(this string value, string customFormat = null) => StringHelpers.IsDateTime(value, customFormat);

        public static bool IsBoolean(this string value) => StringHelpers.IsBoolean(value);

        public static bool IsValidIp(this string value) => StringHelpers.IsValidIp(value);


        public static bool IsBase64Encoded(this string value) => StringHelpers.IsBase64Encoded(value);

        public static bool IsLowerCase(this string value) => StringHelpers.IsLowerCase(value);

        public static bool IsUpperCase(this string value) => StringHelpers.IsUpperCase(value);


        public static Boolean IsValidEmailAddress(this string value) => StringHelpers.IsValidEmailAddress(value);


        public static bool IsGuid(this string value) => StringHelpers.IsGuid(value);

        #endregion

        #region StringTo
        public static String ToNull(this string value) => StringHelpers.ToNull(value);

        public static int ToInt(this string value) => StringHelpers.ToInt(value);

        public static int? ToNullableInt(this string value) => StringHelpers.ToNullableInt(value);

        public static decimal ToDecimal(this string value) => StringHelpers.ToDecimal(value);

        public static decimal? ToNullableDecimal(this string value) => StringHelpers.ToNullableDecimal(value);

        public static float ToFloat(this string value) => StringHelpers.ToFloat(value);


        public static float? ToNullableFloat(this string value) => StringHelpers.ToNullableFloat(value);

        public static long ToLong(this string value) => StringHelpers.ToLong(value);


        public static long? ToNullableLong(this string value) => StringHelpers.ToNullableLong(value);

        public static double ToDouble(this string value) => StringHelpers.ToDouble(value);
        public static double? ToNullableDouble(this string value) => StringHelpers.ToNullableDouble(value);


        public static bool ToBoolean(this string value) => StringHelpers.ToBoolean(value);

        public static DateTime? ToNullableDateTime(this string value, string customFormat = null) => StringHelpers.ToNullableDateTime(value, customFormat);

        public static DateTime ToDateTime(this string value, string customFormat = null) => StringHelpers.ToDateTime(value, customFormat);

        public static string EncodeToBase64(this string value) => StringHelpers.EncodeToBase64(value);

        public static string DecodeFromBase64(this string value) => StringHelpers.DecodeFromBase64(value);

        public static Guid ToGuid(this string value) => StringHelpers.ToGuid(value);



        #endregion

    }
}
