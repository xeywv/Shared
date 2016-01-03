using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public class DBCodeAttribute : Attribute
    {
        private string code;

        public DBCodeAttribute(string code)
        {
            this.code = code;
        }

        /// <summary>
        /// Returns the enumerated values database code.
        /// The enumberated type must have EnumDBCode attributes for each of it members.
        /// </summary>
        /// <typeparam name="T">Enumerated type to convert</typeparam>
        /// <param name="enumValue">Value to convert</param>
        /// <returns>string DB code (or empty string)</returns>
        public static string ToCode<T>(T enumValue)
        {
            return (from p in typeof(T).GetFields()
                    from a in p.GetCustomAttributes(true)
                    where (a is DBCodeAttribute) && p.GetValue(enumValue).Equals(enumValue)
                    select ((DBCodeAttribute)a).code).FirstOrDefault();
        }

        /// <summary>
        /// Returns the enumerated value for a database code
        /// The enumberated type must have EnumDBCode attributes for each of it members.
        /// </summary>
        /// <typeparam name="T">Enumerated type to convert</typeparam>
        /// <param name="code">DB code</param>
        /// <returns>enumerated value (returns first enumerated value if no match found)</returns>
        public static T ToEnum<T>(string code)
        {
            FieldInfo field = (from p in typeof(T).GetFields()
                               from a in p.GetCustomAttributes(true)
                               where (a is DBCodeAttribute) && (((DBCodeAttribute)a).code == code)
                               select p).FirstOrDefault();
            if (field == null)
                return default(T);
            else
                return (T)Enum.Parse(typeof(T), field.Name);
        } 
    }
}
