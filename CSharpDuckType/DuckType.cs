using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpDuckType
{
    public class DuckType
    {
        public static T Cast<T>(object duck)
        {
            return convert<T>(typeof(T), duck);
        }

        private static T convert<T>(Type toType, object duck)
        {
            Type duckType = duck.GetType();
            if (toType.IsAssignableFrom(duckType))
            {
                return (T)duck;
            }
            return default(T);
        }
    }
}
