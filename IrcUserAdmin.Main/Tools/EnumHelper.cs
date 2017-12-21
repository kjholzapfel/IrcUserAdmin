using System;
using System.Linq;

namespace IrcUserAdmin.Tools
{
    public static class EnumHelper
    {
        public static TEnum? ParseEnum<TEnum>(this string value, bool ignoreCase = true) where TEnum : struct,  IComparable, IFormattable, IConvertible
        {
            if ( ! typeof(TEnum).IsEnum) { throw new ArgumentException("TEnum must be an enumerated type"); }
            if (string.IsNullOrEmpty(value)) { return null; }
            TEnum lResult;
            if (Enum.TryParse(value, ignoreCase, out lResult)) { return lResult; }
            return null;
        }
    }
}