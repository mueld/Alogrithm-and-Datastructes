using Bruderer.Core.Domain.Attributes;
using Bruderer.Core.Domain.Models.ModelComponentContainerAggregate;
using Bruderer.Core.Domain.Models.ModelUserAggregate;
using Bruderer.Core.Domain.Models.ValueObjects;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Bruderer.Core.Domain.Models.ModelComponentAggregate
{
    public abstract class ModelComponent : Entity, IAggregateRoot, IModelComponent
    {
        #region fields

        private Guid? _parentModelContainerId = null;
        protected List<FilterTrigger> _filterTriggers = new();
        protected List<IModelComponentObserver> _modelComponentObservers = new();

        #endregion
        #region methods

        public void AddFilterTrigger(FilterTrigger trigger)
        {
            _filterTriggers.Add(trigger);
            _filterTriggers = _filterTriggers
                .Distinct()
                .ToList();
        }
        public void AddFilterTriggers(List<FilterTrigger> triggers)
        {
            _filterTriggers.AddRange(triggers);
            _filterTriggers = _filterTriggers
                .Distinct()
                .ToList();
        }
        public void RemoveFilterTrigger(FilterTrigger trigger)
        {
            if (_filterTriggers.Contains(trigger))
                _filterTriggers.Remove(trigger);
        }
        public IList<FilterTrigger> GetFilterTriggers()
        {
            return _filterTriggers;
        }

        public bool ShouldSerializeModelLink()
        {
            return false;
        }
        public bool ShouldSerializeServiceName()
        {
            return true;
        }
        public bool ShouldSerializeFilters()
        {
            return true;
        }
        public bool ShouldSerializeVersion()
        {
            return false;
        }
        public bool ShouldSerializeLastUpdate()
        {
            return false;
        }
        public bool ShouldSerializeIsPersistent()
        {
            return false;
        }
        public bool ShouldSerializeTwinCAT3Links()
        {
            return false;
        }
        public bool ShouldSerializeOPCUALinks()
        {
            return false;
        }
        public bool ShouldSerializeParentModelContainerId()
        {
            return false;
        }
        public bool ShouldSerializeParentModelContainer()
        {
            return false;
        }

        public void AddObserver(IModelComponentObserver obs)
        {
            if(!_modelComponentObservers.Contains(obs))
            {
                _modelComponentObservers.Add(obs);
                obs.ModelComponentChanged(this);
            }
               
        }
        public void RemoveObserver(IModelComponentObserver obs)
        {
            _modelComponentObservers.Remove(obs);
        }
        public void ClearObserver(IModelComponentObserver obs)
        {
            _modelComponentObservers.Clear();
        }
        protected void NotifyObserver()
        {
            _modelComponentObservers.ForEach(obs => obs.ModelComponentChanged(this));
        }


        #endregion

        #region IModelComponent
        [NotMapped]
        public Guid ModelId { get; set; }
        [NotMapped]
        public KeyValue ModelLink { get; set; } = new();
        [NotMapped]
        public string ServiceName { get; set; } = string.Empty;
        [NotMapped]
        public List<string> Filters { get; set; } = new();
        [NotMapped]
        public uint Version { get; protected set; } = 0;
        [NotMapped]
        public DateTime LastUpdate { get; set; } = DateTime.UtcNow;
        [NotMapped]
        public bool IsPersistent { get; set; } = false;
        [NotMapped]
        public List<ModelTwinCAT3Link> TwinCAT3Links { get; set; } = new();
        [NotMapped]
        public List<ModelOPCUALink> OPCUALinks { get; set; } = new();

        public Guid? ParentModelContainerId
        {
            get
            {
                if (ParentModelContainer != null)
                    return ParentModelContainer.Id;

                return _parentModelContainerId;
            }
            set
            {
                if (ParentModelContainer != null && value != null)
                    ParentModelContainer.Id = (Guid)value;

                _parentModelContainerId = value;
            }
        }
        public ModelComponentContainer? ParentModelContainer { get; set; } = null;

        #endregion
    }
}
