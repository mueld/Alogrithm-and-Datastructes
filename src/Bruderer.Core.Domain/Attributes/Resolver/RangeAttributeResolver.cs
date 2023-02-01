using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace Bruderer.Core.Domain.Attributes.Resolver
{
    public static partial class AttributeResolver
    {
        public static object GetRangeMinimumAttribute(PropertyInfo propertyInfo)
        {

            if (propertyInfo.IsDefined(typeof(RangeAttribute), false))
            {
                var attribute = propertyInfo.GetCustomAttributes(typeof(RangeAttribute), false).FirstOrDefault() as RangeAttribute;
                return attribute.Minimum;
            }

            return null;
        }

        public static object GetRangeMaximumAttribute(PropertyInfo propertyInfo)
        {

            if (propertyInfo.IsDefined(typeof(RangeAttribute), false))
            {
                var attribute = propertyInfo.GetCustomAttributes(typeof(RangeAttribute), false).FirstOrDefault() as RangeAttribute;
                return attribute.Maximum;
            }

            return null;
        }
    }
}
