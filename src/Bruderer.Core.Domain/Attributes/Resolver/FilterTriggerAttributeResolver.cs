using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Bruderer.Core.Domain.Attributes.Resolver
{
    public static partial class AttributeResolver
    {
        public static IEnumerable<FilterTrigger> GetFilterTriggerAttributes(PropertyInfo propertyInfo)
        {
            if (propertyInfo.IsDefined(typeof(FilterTriggerAttribute), false))
            {
                var attributes = propertyInfo.GetCustomAttributes(typeof(FilterTriggerAttribute), false) as FilterTriggerAttribute[];
                return attributes
                    .Select(attribute => attribute.Trigger);
            }

            return new List<FilterTrigger>();
        }
    }
}
