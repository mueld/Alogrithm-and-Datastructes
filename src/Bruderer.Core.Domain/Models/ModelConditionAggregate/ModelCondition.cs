using Bruderer.Core.Domain.Events;
using Bruderer.Core.Domain.Interfaces;
using Bruderer.Core.Domain.Models.ModelComponentAggregate;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bruderer.Core.Domain.Models.ModelConditionAggregate
{
    public abstract class ModelCondition: IModelCondition,IModelComponentObserver
    {
        #region fields

        private HashSet<IModelConditionObserver> observers;
        protected IServiceScopeFactory _ServiceScopeFactory;

        #endregion

        #region ctor 

        public ModelCondition(string modelLink, string conditionName, IServiceScopeFactory serviceScopeFactory)
        {
            TriggerModelLinks = new List<string>() { modelLink};
            observers = new HashSet<IModelConditionObserver>();
            ConditionName = conditionName;
            this._ServiceScopeFactory = serviceScopeFactory;
        }

        public ModelCondition(List<string> modelLinks, string conditionName, IServiceScopeFactory serviceScopeFactory)
        {
            TriggerModelLinks = new List<string>(modelLinks);
            observers = new HashSet<IModelConditionObserver>();
            ConditionName = conditionName;
        }

        #endregion

        #region IModelCondition
        public List<string> TriggerModelLinks { get; protected set; }
         
        public bool IsFulfilled { get; protected set; }

        public string ConditionName { get; protected set; }

        public Guid ModelId { get; set; }
        #endregion

        #region IModelComponentObserver

        public Guid Id { get; private set; } = Guid.NewGuid();
        public abstract void ModelComponentChanged(IModelComponent modelComponent);

        #endregion

        #region methods    
        
        public void AddObserver(IModelConditionObserver obs)
        {
            if (!observers.Add(obs))
            {
                //throw new ArgumentException($"Observer with ID: {obs.Id} has already registered for this condition");
            }
        }
     
        public void RemoveObserver(IModelConditionObserver obs)
        {
            observers.Remove(obs);
        }

        public void RemoveObserver(List<IModelConditionObserver> obs)
        {
            obs.ForEach(o => observers.Remove(o));
        }
       
        public void ClearObservers()
        {
            observers.Clear();
        }
        
        public List<Guid> GetDependendModelComponentIds()
        {
            return observers.Select(obs => obs.Id).ToList();
        }

        protected void NotifyObserver()
        {
            var enumerator = observers.GetEnumerator();
            while(enumerator.MoveNext())
            {
                enumerator.Current.ConditionChanged(this);
            }
           //observers.ForEach(obs => obs.ConditionChanged(this));
        }

        protected void SendEvent()
        {
            using var scope = _ServiceScopeFactory.CreateScope();
            var messageBus = scope.ServiceProvider.GetService<IMessageBus>();

            var metaDataChangedEvent = new ModelVariablesMetadataChangedEvent(ModelId, observers.Select(obs => obs.Id).ToList());
            messageBus.PublishEvent(metaDataChangedEvent);
        }
       
        public override bool Equals(object? obj)
        {
            if(obj is ModelCondition)
            {
                return ((ModelCondition)obj).ConditionName.Equals(this.ConditionName);
            }
            else
            {
                return false;
            }
            
        }

        public override int GetHashCode()
        {
            return TriggerModelLinks.Sum((link) => link.GetHashCode());
        }    
       
        #endregion
    }
}
