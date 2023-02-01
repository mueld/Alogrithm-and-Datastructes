using Bruderer.Core.Domain.Models.ModelComponentContainerAggregate;
using Bruderer.Core.Domain.Models.ModelUserAggregate;
using System;
using System.Collections.Generic;

namespace Bruderer.Core.Domain.Models.ModelComponentAggregate
{
    public interface IModelComponent : IModelContext
    {
        uint Version { get; }
        DateTime LastUpdate { get; }
        bool IsPersistent { get; }
        List<ModelTwinCAT3Link> TwinCAT3Links { get; }
        List<ModelOPCUALink> OPCUALinks { get; }
        Guid? ParentModelContainerId { get; }
        ModelComponentContainer? ParentModelContainer { get; }
    }
}
