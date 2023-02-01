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
        public static List<string> GetEditabilityConditionsAttribute(PropertyInfo propertyInfo)
        {
            if (propertyInfo.IsDefined(typeof(EditabilityConditionsAttribute), false))
            {
                var attribute = propertyInfo.GetCustomAttributes(typeof(EditabilityConditionsAttribute), false).FirstOrDefault() as EditabilityConditionsAttribute;
                if (attribute != null)
                {
                    return attribute.Conditions;
                }
            }

            return new List<string>();
        }

    }
}
