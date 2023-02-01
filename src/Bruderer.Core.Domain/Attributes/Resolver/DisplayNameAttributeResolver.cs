using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Bruderer.Core.Domain.Attributes.Resolver
{
    public static partial class AttributeResolver
    {
        public static string GetDisplayNameAttribute(PropertyInfo propertyInfo)
        {

            if (propertyInfo.IsDefined(typeof(DisplayNameAttribute), false))
            {
                var attribute = propertyInfo.GetCustomAttributes(typeof(DisplayNameAttribute), false).FirstOrDefault() as DisplayNameAttribute;
                return attribute.DisplayName;
            }

            return string.Empty;
        }

        public static string GetDisplayNameAttribute(Type type)
        {

            if (type.IsDefined(typeof(DisplayNameAttribute), false))
            {
                var attribute = type.GetCustomAttributes(typeof(DisplayNameAttribute), false).FirstOrDefault() as DisplayNameAttribute;
                return attribute.DisplayName;
            }

            return string.Empty;
        }
    }
}
