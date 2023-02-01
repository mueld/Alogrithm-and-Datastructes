using Bruderer.Core.Domain.Models.ModelComponentAggregate;
using Bruderer.Core.Domain.Models.ModelIdentityUserAggregate;
using Bruderer.Core.Domain.Models.ModelUserAggregate;
using Bruderer.Core.Domain.Models.ValueObjects;
using System;
using System.Collections.Generic;

namespace Bruderer.Core.Domain.Models.ModelComponentContainerAggregate
{
    public interface IModelComponentContainerCollection : IModelComponent, IModelUserRoleAuthorizationComponent, IModelLocalizationComponent, IModelRepositoryComponent
    {
        new KeyValue ModelLink { get; set; }
        new string ServiceName { get; set; }
        new List<string> Filters { get; set; }

        new uint Version { get; set; }
        new DateTime LastUpdate { get; set; }
        new bool IsPersistent { get; set; }
        new List<ModelTwinCAT3Link> TwinCAT3Links { get; set; }
        new List<ModelOPCUALink> OPCUALinks { get; set; }
        new Guid? ParentModelContainerId { get; set; }
        new ModelComponentContainer? ParentModelContainer { get; set; }

        new ModelIdentityUserRoleEnumeration VisibilityAuthorization { get; set; }
        new ModelIdentityUserRoleEnumeration ManipulationAuthorization { get; set; }

        new LocalizableValue Display { get; set; }
        new LocalizableValue Description { get; set; }

        new bool IsRepositoryClient { get; set; }
        new string RepositoryTypeName { get; set; }
        new Guid RepositoryId { get; set; }
        new string RepositoryLink { get; set; }

        int Count { get; }
        int ContainerCountLimit { get; }
        Type ContainerType { get; }
        IEnumerable<IModelComponentContainer> Containers { get; }

        void AddModelContainer();
        void RemoveModelContainer();
    }
}
