using Bruderer.Core.Domain.Models.ModelIdentityUserAggregate;
using System;

namespace Bruderer.Core.Domain.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class VisibilityAuthorizationAttribute : Attribute
    {
        #region ctor

        public VisibilityAuthorizationAttribute(ModelIdentityUserRoleEnumeration role)
        {
            Role = role;
        }

        #endregion
        #region props

        public ModelIdentityUserRoleEnumeration Role { get; set; } = ModelIdentityUserRoleEnumeration.Undefined;

        #endregion
    }
}
