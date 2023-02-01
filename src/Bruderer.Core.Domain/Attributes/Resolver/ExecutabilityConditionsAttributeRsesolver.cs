using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;


namespace Bruderer.Core.Domain.Attributes.Resolver
{
    public static partial class AttributeResolver
    {
        public static List<string> GetExecutabilityConditionsAttribute(PropertyInfo propertyInfo)
        {
            if (propertyInfo.IsDefined(typeof(ExecutabilityCondtionsAttribute), false))
            {
                var attribute = propertyInfo.GetCustomAttributes(typeof(ExecutabilityCondtionsAttribute), false).FirstOrDefault() as ExecutabilityCondtionsAttribute;
                if (attribute != null)
                {
                    return attribute.Conditions;
                }
            }

            return new List<string>();
        }
    }
}
