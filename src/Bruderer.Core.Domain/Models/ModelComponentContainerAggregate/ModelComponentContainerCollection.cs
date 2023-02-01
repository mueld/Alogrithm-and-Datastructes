using Bruderer.Core.Domain.Models.ModelIdentityUserAggregate;
using Bruderer.Core.Domain.Models.ModelUserAggregate;
using Bruderer.Core.Domain.Models.ValueObjects;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Bruderer.Core.Domain.Models.ModelComponentContainerAggregate
{
    public class ModelComponentContainerCollection<T> : Collection<T>, IModelComponentContainerCollection
        where T : ModelComponentContainer
    {
        #region fields

        private bool _InsertRealValues = false;
        private bool _FillingDefaultSize = false;

        #endregion
        #region ctor

        public ModelComponentContainerCollection()
        {
            Id = Guid.NewGuid();
        }

        public ModelComponentContainerCollection(int elementCountLimit = 0)
        {
            Id = Guid.NewGuid();
            ContainerCountLimit = elementCountLimit;

            if (ContainerCountLimit > 0)
            {
                _FillingDefaultSize = true;

                for (int i = 0; i < ContainerCountLimit; i++)
                {
                    Add((T)Activator.CreateInstance(typeof(T)));
                }

                _FillingDefaultSize = false;
            }
        }

        #endregion
        #region methods

        protected override void InsertItem(int index, T item)
        {
            item.IsEnumerable = true;
            if (item.EnumerationIndex < 0)
                item.EnumerationIndex = index;

            if (!_FillingDefaultSize && !_InsertRealValues)
            {
                _InsertRealValues = true;

                Clear();
                index = 0;
            }
            if (ContainerCountLimit > 0)
            {
                if (Count < ContainerCountLimit)
                    base.InsertItem(index, item);
            }
            else
            {
                base.InsertItem(index, item);
            }

            // Replace order by the index of the item
            var orderedItems = Items.OrderBy(item => item.EnumerationIndex).ToList();
            for (int i = 0; i < orderedItems.Count; i++)
                SetItem(i, orderedItems[i]);
        }

        #endregion

        #region IModelContainerCollection

        public Guid Id { get; set; }
        public Guid ModelId { get; set; }

        [NotMapped]
        public KeyValue ModelLink { get; set; } = new();
        [NotMapped]
        public string ServiceName { get; set; } = string.Empty;
        [NotMapped]
        public List<string> Filters { get; set; } = new();
        [NotMapped]
        public uint Version { get; set; } = 0;
        [NotMapped]
        public DateTime LastUpdate { get; set; } = DateTime.UtcNow;
        [NotMapped]
        public bool IsPersistent { get; set; } = false;

        [NotMapped]
        public LocalizableValue Display { get; set; } = new();
        [NotMapped]
        public LocalizableValue Description { get; set; } = new();

        [NotMapped]
        public ModelIdentityUserRoleEnumeration VisibilityAuthorization { get; set; } = ModelIdentityUserRoleEnumeration.Undefined;
        [NotMapped]
        public ModelIdentityUserRoleEnumeration ManipulationAuthorization { get; set; } = ModelIdentityUserRoleEnumeration.Undefined;

        [NotMapped]
        public List<ModelTwinCAT3Link> TwinCAT3Links { get; set; } = new();
        [NotMapped]
        public List<ModelOPCUALink> OPCUALinks { get; set; } = new();

        [NotMapped]
        public bool IsRepositoryClient { get; set; } = false;
        [NotMapped]
        public string RepositoryLink { get; set; } = string.Empty;
        [NotMapped]
        public string RepositoryTypeName { get; set; } = string.Empty;
        [NotMapped]
        public string RepositoryPartTypeName { get; set; } = string.Empty;
        [NotMapped]
        public Guid RepositoryId { get; set; } = Guid.Empty;
        [NotMapped]
        public bool IsRepositoryRootContainer { get; set; } = false;

        [NotMapped]
        public int ContainerCountLimit { get; set; }
        [NotMapped]
        public Type ContainerType { get { return typeof(T); } }
        [NotMapped]
        public  IEnumerable<IModelComponentContainer> Containers { get { return this; } }

        public Guid? ParentModelContainerId
        {
            get
            {
                if (ParentModelContainer != null)
                    return ParentModelContainer.Id;

                return Guid.Empty;
            }
            set
            {
                if (ParentModelContainer != null && value != null)
                    ParentModelContainer.Id = (Guid)value;
            }
        }
        public ModelComponentContainer? ParentModelContainer { get; set; } = null;

        public void AddModelContainer()
        {
            var newContainerElement = (T)Activator.CreateInstance(ContainerType);
            if (newContainerElement == null)
            {
                return;
            }

            var newContainerRepositoryElement = newContainerElement as ISeedableModelContainer;
            if (newContainerRepositoryElement != null)
            {
                newContainerRepositoryElement.DataSeeding();
            }

            Add(newContainerElement);
        }

        public void RemoveModelContainer()
        {
            if (Count > 0)
                RemoveAt(Count - 1);
        }

        #endregion
    }
}
