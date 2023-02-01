using System.Linq;
using System.Reflection;

namespace Bruderer.Core.Domain.Attributes.Resolver
{
    public static partial class AttributeResolver
    {
        public static TwinCAT3IgnoreAttribute GetTwinCAT3IgnoreAttribute(PropertyInfo propertyInfo)
        {

            if (propertyInfo.IsDefined(typeof(TwinCAT3IgnoreAttribute), false))
            {
                return propertyInfo.GetCustomAttributes(typeof(TwinCAT3IgnoreAttribute), false).FirstOrDefault() as TwinCAT3IgnoreAttribute;
            }

            return null;
        }
    }
}
