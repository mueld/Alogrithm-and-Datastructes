using System;

namespace Bruderer.Core.Domain.Models.ModelComponentAggregate
{
    public interface IModelComponentObserver
    {
        Guid Id { get; }
        public void ModelComponentChanged(IModelComponent modelComponent);
    }
}
