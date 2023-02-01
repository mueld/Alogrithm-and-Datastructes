using Bruderer.Core.Domain.Models.ModelIdentityUserAggregate;
using System.Linq;
using System.Reflection;

namespace Bruderer.Core.Domain.Attributes.Resolver
{
    public static partial class AttributeResolver
    {
        public static ModelIdentityUserRoleEnumeration GetManipulationAuthorizationAttribute(PropertyInfo propertyInfo)
        {
            if (propertyInfo.IsDefined(typeof(ManipulationAuthorizationAttribute), false))
            {
                var attribute = propertyInfo.GetCustomAttributes(typeof(ManipulationAuthorizationAttribute), false).FirstOrDefault() as ManipulationAuthorizationAttribute;
                if (attribute != null)
                {
                    return attribute.Role;
                }
            }

            return ModelIdentityUserRoleEnumeration.Undefined;
        }
    }
}
