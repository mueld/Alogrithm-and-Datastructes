using System.Collections.Generic;

namespace Bruderer.Core.Domain.Models.ModelComponentContainerAggregate
{
    public interface IRepositoryModelContainer : ISeedableModelContainer
    {
        RepositoryCreationPolicyEnumeration RepositoryCreationPolicy { get; set; }
        List<string> RepositoryRelatedModelLinks { get; set; }
    }
}
