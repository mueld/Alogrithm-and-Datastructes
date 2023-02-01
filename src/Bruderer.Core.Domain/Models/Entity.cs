using Bruderer.Core.Domain.Messaging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bruderer.Core.Domain.Models
{
    public abstract class Entity
    {
        #region fields

        private List<Event> _DomainEvents;

        #endregion
        #region ctor

        protected Entity()
        {
            Id = Guid.NewGuid();
        }

        #endregion
        #region props

        public virtual Guid Id { get; set; }

        [NotMapped]
        public IReadOnlyCollection<Event> DomainEvents => _DomainEvents?.AsReadOnly();

        #endregion
        #region methods

        public virtual bool ShouldSerializeId()
        {
            return true;
        }

        public bool ShouldSerializeDomainEvents()
        {
            return false;
        }

        public void AddDomainEvent(Event domainEvent)
        {
            _DomainEvents ??= new List<Event>();
            _DomainEvents.Add(domainEvent);
        }

        public void RemoveDomainEvent(Event domainEvent)
        {
            _DomainEvents?.Remove(domainEvent);
        }

        public void ClearDomainEvents()
        {
            _DomainEvents?.Clear();
        }

        #endregion
        #region behaviours

        public override bool Equals(object obj)
        {
            var compareTo = obj as Entity;

            if (ReferenceEquals(this, compareTo)) return true;
            if (ReferenceEquals(null, compareTo)) return false;

            return Id.Equals(compareTo.Id);
        }

        public static bool operator ==(Entity a, Entity b)
        {
            if (a is null && b is null)
                return true;

            if (a is null || b is null)
                return false;

            return a.Equals(b);
        }

        public static bool operator !=(Entity a, Entity b)
        {
            return !(a == b);
        }

        public override int GetHashCode()
        {
            return (GetType().GetHashCode() ^ 93) + Id.GetHashCode();
        }

        public override string ToString()
        {
            return $"{GetType().Name} [Id={Id}]";
        }

        #endregion
    }
}
