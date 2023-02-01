using Bruderer.Core.Domain.Models.ModelIdentityUserAggregate;

namespace Bruderer.Core.Domain.Models.ModelComponentAggregate
{
    public interface IModelUserRoleAuthorizationComponent
    {
        ModelIdentityUserRoleEnumeration VisibilityAuthorization { get; }
        ModelIdentityUserRoleEnumeration ManipulationAuthorization { get; }
    }
}
