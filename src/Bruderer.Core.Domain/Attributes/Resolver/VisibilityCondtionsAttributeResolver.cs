using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Bruderer.Core.Domain.Attributes.Resolver
{
    public static partial class AttributeResolver
    {
        public static List<string> GetVisibilityConditionsAttribute(PropertyInfo propertyInfo)
        {
            if (propertyInfo.IsDefined(typeof(VisibilityCondtionsAttribute), false))
            {
                var attribute = propertyInfo.GetCustomAttributes(typeof(VisibilityCondtionsAttribute), false).FirstOrDefault() as VisibilityCondtionsAttribute;
                if (attribute != null)
                {
                    return attribute.Conditions;
                }
            }

            return new List<string>();
        }
    }
}
