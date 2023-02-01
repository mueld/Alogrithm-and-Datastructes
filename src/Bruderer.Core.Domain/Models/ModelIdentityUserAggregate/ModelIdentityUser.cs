using Microsoft.AspNetCore.Identity;

namespace Bruderer.Core.Domain.Models.ModelIdentityUserAggregate
{
    public class ModelIdentityUser : IdentityUser, IAggregateRoot
    {
        #region ctor

        public ModelIdentityUser()
        {
        }

        #endregion
    }
}
