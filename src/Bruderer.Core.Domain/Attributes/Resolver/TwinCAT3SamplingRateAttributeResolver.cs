using System;
using System.Linq;
using System.Reflection;

namespace Bruderer.Core.Domain.Attributes.Resolver
{
    public static partial class AttributeResolver
    {
        public static TwinCAT3SamplingRateAttribute GetTwinCAT3SamplingRateAttribute(PropertyInfo propertyInfo)
        {

            if (propertyInfo.IsDefined(typeof(TwinCAT3SamplingRateAttribute), false))
            {
                return propertyInfo.GetCustomAttributes(typeof(TwinCAT3SamplingRateAttribute), false).FirstOrDefault() as TwinCAT3SamplingRateAttribute;
            }

            return null;
        }

        public static TwinCAT3SamplingRateAttribute GetTwinCAT3SamplingRateAttribute(Type type)
        {

            if (type.IsDefined(typeof(TwinCAT3SamplingRateAttribute), false))
            {
                return type.GetCustomAttributes(typeof(TwinCAT3SamplingRateAttribute), false).FirstOrDefault() as TwinCAT3SamplingRateAttribute;
            }

            return null;
        }
    }
}
