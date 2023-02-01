using System;
using System.Linq;
using System.Reflection;

namespace Bruderer.Core.Domain.Attributes.Resolver
{
    public static partial class AttributeResolver
    {
        public static string GetLocalizationNamespaceAttribute(PropertyInfo propertyInfo)
        {

            if (propertyInfo.IsDefined(typeof(LocalizationNamespaceAttribute), false))
            {
                var attribute = propertyInfo.GetCustomAttributes(typeof(LocalizationNamespaceAttribute), false).FirstOrDefault() as LocalizationNamespaceAttribute;
                if (attribute != null)
                {
                    return attribute.NamespaceName;
                }
            }

            return string.Empty;
        }

        public static string GetLocalizationNamespaceAttribute(Type type)
        {

            if (type.IsDefined(typeof(LocalizationNamespaceAttribute), false))
            {
                var attribute = type.GetCustomAttributes(typeof(LocalizationNamespaceAttribute), false).FirstOrDefault() as LocalizationNamespaceAttribute;
                return attribute.NamespaceName;
            }

            return string.Empty;
        }
    }
}
