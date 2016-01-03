using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public static class StringBuilderExtension
    {
        public static StringBuilder AppendLine(this StringBuilder stringBuilder, string formatter, params object[] args)
        {
            return stringBuilder.AppendLine(string.Format(formatter, args));
        }
    }
}
