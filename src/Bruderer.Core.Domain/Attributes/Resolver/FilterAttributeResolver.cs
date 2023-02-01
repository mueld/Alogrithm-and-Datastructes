using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Bruderer.Core.Domain.Attributes.Resolver
{
    public static partial class AttributeResolver
    {
        public static IList<string> GetFilterAttribute(PropertyInfo propertyInfo)
        {
            if (propertyInfo.IsDefined(typeof(FilterAttribute), false))
            {
                var attribute = propertyInfo.GetCustomAttributes(typeof(FilterAttribute), false).FirstOrDefault() as FilterAttribute;
                if (attribute != null)
                {
                    return attribute.Filters;
                }
            }

            return new List<string>();
        }
    }
}
