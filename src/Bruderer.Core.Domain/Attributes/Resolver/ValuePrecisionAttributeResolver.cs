using System;
using System.Linq;
using System.Reflection;

namespace Bruderer.Core.Domain.Attributes.Resolver
{
    public static partial class AttributeResolver
    {
        public static int? GetValuePrecision(PropertyInfo propertyInfo)
        {

            if (propertyInfo.IsDefined(typeof(ValuePrecisionAttribute), false))
            {
                var attribute = propertyInfo.GetCustomAttributes(typeof(ValuePrecisionAttribute), false).FirstOrDefault() as ValuePrecisionAttribute;
                return attribute.ValuePrecision;
            }

            return null;
        }

        public static double? GetValuePrecision(Type type)
        {

            if (type.IsDefined(typeof(ValuePrecisionAttribute), false))
            {
                var attribute = type.GetCustomAttributes(typeof(ValuePrecisionAttribute), false).FirstOrDefault() as ValuePrecisionAttribute;
                return attribute.ValuePrecision;
            }

            return null;
        }
    }
}
