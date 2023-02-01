using System;
using System.Linq;
using System.Reflection;

namespace Bruderer.Core.Domain.Attributes.Resolver
{
    public static partial class AttributeResolver
    {
        public static string GetModePathAttribute(PropertyInfo propertyInfo)
        {

            if (propertyInfo.IsDefined(typeof(ModelPathAttribute), false))
            {
                var attribute = propertyInfo.GetCustomAttributes(typeof(ModelPathAttribute), false).FirstOrDefault() as ModelPathAttribute;
                return attribute.LinkPath;
            }

            return string.Empty;
        }

        public static string GetModelPathAttribute(Type type)
        {

            if (type.IsDefined(typeof(ModelPathAttribute), false))
            {
                var attribute = type.GetCustomAttributes(typeof(ModelPathAttribute), false).FirstOrDefault() as ModelPathAttribute;
                return attribute.LinkPath;
            }

            return string.Empty;
        }
    }
}
