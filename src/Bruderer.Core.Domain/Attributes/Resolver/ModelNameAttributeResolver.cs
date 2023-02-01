using System;
using System.Linq;
using System.Reflection;

namespace Bruderer.Core.Domain.Attributes.Resolver
{
    public static partial class AttributeResolver
    {
        public static string GetModeNameAttribute(PropertyInfo propertyInfo)
        {

            if (propertyInfo.IsDefined(typeof(ModelNameAttribute), false))
            {
                var attribute = propertyInfo.GetCustomAttributes(typeof(ModelNameAttribute), false).FirstOrDefault() as ModelNameAttribute;
                return attribute.LinkName;
            }

            return string.Empty;
        }

        public static string GetModeNameAttribute(Type type)
        {

            if (type.IsDefined(typeof(ModelNameAttribute), false))
            {
                var attribute = type.GetCustomAttributes(typeof(ModelNameAttribute), false).FirstOrDefault() as ModelNameAttribute;
                return attribute.LinkName;
            }

            return string.Empty;
        }
    }
}
