using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Bruderer.Core.Domain.Attributes.Resolver
{
    public static partial class AttributeResolver
    {
        public static string GetDescriptionAttribute(PropertyInfo propertyInfo)
        {

            if (propertyInfo.IsDefined(typeof(DescriptionAttribute), false))
            {
                var attribute = propertyInfo.GetCustomAttributes(typeof(DescriptionAttribute), false).FirstOrDefault() as DescriptionAttribute;
                return attribute.Description;
            }

            return string.Empty;
        }

        public static string GetDescriptionAttribute(Type type)
        {

            if (type.IsDefined(typeof(DescriptionAttribute), false))
            {
                var attribute = type.GetCustomAttributes(typeof(DescriptionAttribute), false).FirstOrDefault() as DescriptionAttribute;
                return attribute.Description;
            }

            return string.Empty;
        }
    }
}
