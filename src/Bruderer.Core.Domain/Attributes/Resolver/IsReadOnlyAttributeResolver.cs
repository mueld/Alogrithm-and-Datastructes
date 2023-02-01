using System.Linq;
using System.Reflection;

namespace Bruderer.Core.Domain.Attributes.Resolver
{
    public static partial class AttributeResolver
    {
        public static bool? GetIsReadyOnlyAttribute(PropertyInfo propertyInfo)
        {
            if (propertyInfo.IsDefined(typeof(IsReadOnlyAttribute), false))
            {
                var attribute = propertyInfo.GetCustomAttributes(typeof(IsReadOnlyAttribute), false).FirstOrDefault() as IsReadOnlyAttribute;
                if (attribute != null)
                {
                    return attribute.IsEnabled;
                }
            }

            return null;
        }
    }
}
