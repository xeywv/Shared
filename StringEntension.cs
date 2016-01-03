using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public static class StringEntension
    {
        public static bool ContainsNoCase(this string str, string value)
        {
            if (str == null)
                return false;
            return str.IndexOf(value, 0, StringComparison.InvariantCultureIgnoreCase) >= 0;
        }

        public static string SafeSubString(this string val, int startPos, int length)
        {
            if (val == null)
                return null;

            startPos = Math.Min(startPos, val.Length);
            length   = Math.Min(length, val.Length - startPos);
            return val.Substring(startPos, length); 
        }
    }
}
