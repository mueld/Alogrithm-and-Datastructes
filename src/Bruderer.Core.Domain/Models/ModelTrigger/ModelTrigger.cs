using Bruderer.Core.Domain.Models.ModelComponentAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bruderer.Core.Domain.Models.ModelTrigger
{
    public abstract class ModelTrigger : IModelComponentObserver
    {
        private readonly HashSet<IModelTriggerHandler> _handler = new();


        public Guid Id { get; private set; } = new Guid();

        public void AddHandler(IModelTriggerHandler handler)
        {
            _handler.Add(handler);
        }

        public void RemoveHandler(IModelTriggerHandler handler)
        {
            _handler.Remove(handler);
        }

        public abstract void ModelComponentChanged(IModelComponent modelComponent);

    }
}
