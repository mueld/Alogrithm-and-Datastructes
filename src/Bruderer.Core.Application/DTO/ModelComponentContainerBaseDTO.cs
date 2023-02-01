using Bruderer.Core.Domain.Models.ModelComponentContainerAggregate;
using Bruderer.Core.Domain.Models.ModelIdentityUserAggregate;
using Bruderer.Core.Domain.Models.ValueObjects;
using System.Collections.Generic;

namespace Bruderer.Core.Application.DTO
{
    // OFI -> Check for auto mapping solutions as https://github.com/AutoMapper/AutoMapper
    public class ModelComponentContainerBaseDTO
    {
        #region ctor

        public ModelComponentContainerBaseDTO()
        { }

        public ModelComponentContainerBaseDTO(IModelComponentContainer modelComponentContainer)
        {
            Take(modelComponentContainer);
        }

        #endregion
        #region props

        public string Id { get; set; } = string.Empty;
        public KeyValue ModelLink { get; set; } = new KeyValue();
        public string ServiceName { get; set; } = string.Empty;
        public List<string> Filters { get; set; } = new List<string>();
        public bool IsPersistent { get; set; } = false;

        public string ParentModelContainerId { get; set; } = string.Empty;
        public string ParentModelContainerModelKey { get; set; } = string.Empty;

        public LocalizableValue Display { get; set; } = new LocalizableValue();
        public LocalizableValue Description { get; set; } = new LocalizableValue();

        public ModelIdentityUserRoleEnumeration VisibilityAuthorization { get; set; } = ModelIdentityUserRoleEnumeration.Undefined;
        public ModelIdentityUserRoleEnumeration ManipulationAuthorization { get; set; } = ModelIdentityUserRoleEnumeration.Undefined;

        public bool IsEnumerableContainer { get; set; } = false;
        public bool IsEnumerable { get; set; } = false;
        public int EnumerationIndex { get; set; } = -1;
        public int EnumerationCount { get; set; } = -1;

        public bool IsRepositoryClient { get; set; } = false;
        public string RepositoryName { get; set; } = string.Empty;
        public string RepositoryId { get; set; } = string.Empty;
        public string RepositoryLink { get; set; } = string.Empty;
        public bool IsRepositoryRootContainer { get; set; } = false;

        #endregion
        #region methods

        private void Take(IModelComponentContainer modelComponentContainer)
        {
            Id = modelComponentContainer.Id.ToString();
            ModelLink.Path = modelComponentContainer.ModelLink.Path;
            ModelLink.Name = modelComponentContainer.ModelLink.Name;
            ServiceName = modelComponentContainer.ServiceName;
            Filters = new List<string>(modelComponentContainer.Filters);
            IsPersistent = modelComponentContainer.IsPersistent;

            if (modelComponentContainer.ParentModelContainer != null)
            {
                ParentModelContainerId = modelComponentContainer.ParentModelContainer.Id.ToString();
                ParentModelContainerModelKey = modelComponentContainer.ParentModelContainer.ModelLink.Key;
            }
            
            Display.KeyNamespace = modelComponentContainer.Display.KeyNamespace;
            Display.Value = modelComponentContainer.Display.Value;
            Display.Link.Path = modelComponentContainer.Display.Link.Path;
            Display.Link.Name = modelComponentContainer.Display.Link.Name;
            Display.DynamicValueDictionary = new Dictionary<string, string>(modelComponentContainer.Display.DynamicValueDictionary);

            Description.KeyNamespace = modelComponentContainer.Description.KeyNamespace;
            Description.Value = modelComponentContainer.Description.Value;
            Description.Link.Path = modelComponentContainer.Description.Link.Path;
            Description.Link.Name = modelComponentContainer.Description.Link.Name;
            Description.DynamicValueDictionary = new Dictionary<string, string>(modelComponentContainer.Description.DynamicValueDictionary);

            VisibilityAuthorization = modelComponentContainer.VisibilityAuthorization;
            ManipulationAuthorization = modelComponentContainer.ManipulationAuthorization;

            IsEnumerableContainer = modelComponentContainer.IsEnumerableContainer;
            IsEnumerable = modelComponentContainer.IsEnumerable;
            EnumerationCount = modelComponentContainer.EnumerationCount;
            EnumerationIndex = modelComponentContainer.EnumerationIndex;

            IsRepositoryClient = modelComponentContainer.IsRepositoryClient;
            RepositoryName = modelComponentContainer.RepositoryTypeName;
            RepositoryId = modelComponentContainer.RepositoryId.ToString();
            RepositoryLink = modelComponentContainer.RepositoryLink;
            IsRepositoryRootContainer = modelComponentContainer.IsRepositoryRootContainer;
        }

        #endregion
    }
}
