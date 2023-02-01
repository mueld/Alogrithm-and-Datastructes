using Bruderer.Core.Domain.Models.ModelIdentityUserAggregate;
using System.Linq;
using System.Reflection;

namespace Bruderer.Core.Domain.Attributes.Resolver
{
    public static partial class AttributeResolver
    {
        public static ModelIdentityUserRoleEnumeration GetVisibilityAuthorizationAttribute(PropertyInfo propertyInfo)
        {
            if (propertyInfo.IsDefined(typeof(VisibilityAuthorizationAttribute), false))
            {
                var attribute = propertyInfo.GetCustomAttributes(typeof(VisibilityAuthorizationAttribute), false).FirstOrDefault() as VisibilityAuthorizationAttribute;
                if (attribute != null)
                {
                    return attribute.Role;
                }
            }

            return ModelIdentityUserRoleEnumeration.Undefined;
        }
    }
}
