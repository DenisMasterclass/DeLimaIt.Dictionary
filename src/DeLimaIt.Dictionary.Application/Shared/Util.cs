
using System.Globalization;

namespace DeLimaIt.Dictionary.Application.Shared
{
    public static class Util
    {
        public static string ToStringFromDictionary(this string value,string parameterName,Dictionary<string, Dictionary<string, string>> dicParameters)
        {
            string parameterValue = "";
            if (dicParameters.TryGetValue(parameterName, out var dicParameter))
            {
                dicParameter.TryGetValue(value, out parameterValue);
            }
            return parameterValue;
        }

        public static string ToStringFromDictionary(this int value, string parameterName, Dictionary<string, Dictionary<string, string>> dicParameters)
        {
            string parameterValue = "";
            if (dicParameters.TryGetValue(parameterName, out var dicParameter))
            {
                dicParameter.TryGetValue(value.ToString(), out parameterValue);
            }
            return parameterValue;
        }

        public static string ToStringFromDictionaryOrDefault(this string value, string parameterName, Dictionary<string, Dictionary<string, string>> dicParameters)
        {
            string parameterValue = "";
            if (dicParameters.TryGetValue(parameterName, out var dicParameter))
            {
                if (!dicParameter.TryGetValue(value, out parameterValue))
                {
                    parameterValue = value;

                }
            }
            return parameterValue;
        }
        public static string ToBrString(this decimal value)
        {
            return value.ToString("0.####################", CultureInfo.GetCultureInfo("pt-BR"));
        }
        public static string ToGbString(this decimal value)
        {
            return value.ToString("0.####################", CultureInfo.GetCultureInfo("en-GB"));
        }

    }
}
