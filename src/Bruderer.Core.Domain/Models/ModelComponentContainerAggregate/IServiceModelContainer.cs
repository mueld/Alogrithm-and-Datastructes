using Bruderer.Core.Domain.Models.ModelVariableAggregate;
using System;
using System.Collections.Generic;

namespace Bruderer.Core.Domain.Models.ModelComponentContainerAggregate
{
    public interface IServiceModelContainer : IModelComponentContainer
    {
        public bool IsEnabled { get; }
    }
}
