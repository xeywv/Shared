using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public static class Converter
    {
        public static T ChangeType<T>(string value)
        {
            Type type = typeof(T);
            object result;

            if (type.Name.StartsWith("Nullable"))
            {
                if (string.IsNullOrEmpty(value))
                {
                    result = null;
                    return (T)result;
                }
                else
                    type = type.GenericTypeArguments[0];
            }

            result = Convert.ChangeType(value, type);
            return (T)result;
        }
    }
}
