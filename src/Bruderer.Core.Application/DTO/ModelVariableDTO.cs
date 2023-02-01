using Bruderer.Core.Domain.Models.ModelIdentityUserAggregate;
using Bruderer.Core.Domain.Models.ModelUserAggregate;
using Bruderer.Core.Domain.Models.ModelVariableAggregate;
using Bruderer.Core.Domain.Models.ValueObjects;
using System;
using System.Collections.Generic;

#nullable enable

namespace Bruderer.Core.Application.DTO
{
    // OFI -> Check for auto mapping solutions as https://github.com/AutoMapper/AutoMapper
    public class ModelVariableDTO
    {
        #region ctor

        public ModelVariableDTO()
        { }

        public ModelVariableDTO(ModelVariableBase modelVariable)
        {
            if (modelVariable != null)
#pragma warning disable CS8604 // Possible null reference argument.
                Take(modelVariable as IModelVariable);
#pragma warning restore CS8604 // Possible null reference argument.
        }

        public ModelVariableDTO(IModelVariable modelVariable)
        {
            Take(modelVariable);
        }

        #endregion
        #region props

        public string Id { get; set; } = string.Empty;
        public KeyValue ModelLink { get; set; } = new KeyValue();
        public string ServiceName { get; set; } = string.Empty;
        public List<string> Filters { get; set; } = new List<string>();
        public uint Version { get; set; } = 0;
        public DateTime LastUpdate { get; set; } = DateTime.UtcNow;
        public bool IsPersistent { get; set; } = false;
        public List<ModelTwinCAT3Link> TwinCAT3Links { get; set; } = new();
        public List<ModelOPCUALink> OPCUALinks { get; set; } = new();

        public string ParentModelContainerId { get; set; } = string.Empty;
        public string ParentModelContainerModelKey { get; set; } = string.Empty;

        public LocalizableValue Display { get; set; } = new LocalizableValue();
        public LocalizableValue Description { get; set; } = new LocalizableValue();

        public ModelIdentityUserRoleEnumeration VisibilityAuthorization { get; set; } = ModelIdentityUserRoleEnumeration.Undefined;
        public ModelIdentityUserRoleEnumeration ManipulationAuthorization { get; set; } = ModelIdentityUserRoleEnumeration.Undefined;

        public bool IsRepositoryClient { get; set; } = false;
        public string RepositoryName { get; set; } = string.Empty;
        public string RepositoryPartName { get; set; } = string.Empty;
        public string RepositoryId { get; set; } = string.Empty;
        public string RepositoryLink { get; set; } = string.Empty;

        public UnitValue? EngineeringUnit { get; set; } = null; 
        public bool IsReadOnly { get; set; } = false;
        public bool IsVisible { get; set; } = false;
        public bool IsInvalid { get; set; } = false;
        public string ValueType { get; set; } = string.Empty;
        public int? ValuePrecision { get; set; } = null;
        public string ValueEnumerationKey { get; set; } = string.Empty;
        public object? ValueMinimum { get; set; }
        public object? ValueMaximum { get; set; }
        public object? Value { get; set; }

        #endregion
        #region methods

        public IModelVariableDEO GetDEO()
        {
            var modelVariableDEO = new ModelVariableDEO();

            modelVariableDEO.ModelContext.Id = Guid.Parse(Id);
            modelVariableDEO.ModelContext.ModelLink.Path = ModelLink.Path;
            modelVariableDEO.ModelContext.ModelLink.Name = ModelLink.Name;
            modelVariableDEO.ModelContext.ServiceName = ServiceName;
            modelVariableDEO.ModelContext.Filters = new List<string>(Filters);

            modelVariableDEO.RepositoryContext.IsRepositoryClient = IsRepositoryClient;
            modelVariableDEO.RepositoryContext.RepositoryTypeName = RepositoryName;
            modelVariableDEO.RepositoryContext.RepositoryPartTypeName = RepositoryPartName;
            modelVariableDEO.RepositoryContext.RepositoryId = Guid.Parse(RepositoryId);
            modelVariableDEO.RepositoryContext.RepositoryLink = RepositoryLink;

            modelVariableDEO.Payload.Value = Value;
            modelVariableDEO.Payload.ValueMaximum = ValueMaximum;
            modelVariableDEO.Payload.ValueMinimum = ValueMinimum;

            return modelVariableDEO;
        }

        private void Take(IModelVariable modelVariable)
        {
            Id = modelVariable.Id.ToString();
            ModelLink.Path = modelVariable.ModelLink.Path;
            ModelLink.Name = modelVariable.ModelLink.Name;
            ServiceName = modelVariable.ServiceName;
            Filters = new List<string>(modelVariable.Filters);
            Version = modelVariable.Version;
            LastUpdate = modelVariable.LastUpdate;
            IsPersistent = modelVariable.IsPersistent;
            TwinCAT3Links = new List<ModelTwinCAT3Link>(modelVariable.TwinCAT3Links);
            OPCUALinks = new List<ModelOPCUALink>(modelVariable.OPCUALinks);

            if (modelVariable.ParentModelContainer != null)
            {
                ParentModelContainerId = modelVariable.ParentModelContainer.Id.ToString();
                ParentModelContainerModelKey = modelVariable.ParentModelContainer.ModelLink.Key;
            }

            Display.KeyNamespace = modelVariable.Display.KeyNamespace;
            Display.Value = modelVariable.Display.Value;
            Display.Link.Path = modelVariable.Display.Link.Path;
            Display.Link.Name = modelVariable.Display.Link.Name;
            Display.DynamicValueDictionary = new Dictionary<string, string>(modelVariable.Display.DynamicValueDictionary);

            Description.KeyNamespace = modelVariable.Description.KeyNamespace;
            Description.Value = modelVariable.Description.Value;
            Description.Link.Path = modelVariable.Description.Link.Path;
            Description.Link.Name = modelVariable.Description.Link.Name;
            Description.DynamicValueDictionary = new Dictionary<string, string>(modelVariable.Description.DynamicValueDictionary);

            VisibilityAuthorization = modelVariable.VisibilityAuthorization;
            ManipulationAuthorization = modelVariable.ManipulationAuthorization;

            IsRepositoryClient = modelVariable.IsRepositoryClient;
            RepositoryName = modelVariable.RepositoryTypeName;
            RepositoryPartName = modelVariable.RepositoryPartTypeName;
            RepositoryId = modelVariable.RepositoryId.ToString();
            RepositoryLink = modelVariable.RepositoryLink;

            if (modelVariable.EngineeringUnit != null)
            {
                EngineeringUnit = new();
                EngineeringUnit.Description = modelVariable.EngineeringUnit.Description;
                EngineeringUnit.DisplayName = modelVariable.EngineeringUnit.DisplayName;
                EngineeringUnit.UNECECode = modelVariable.EngineeringUnit.UNECECode;
                EngineeringUnit.UnitID = modelVariable.EngineeringUnit.UnitID;
            }    
            IsReadOnly = modelVariable.IsReadOnly;
            IsVisible = modelVariable.IsVisible;
            IsInvalid = modelVariable.IsInvalid;
            ValueType = modelVariable.ValueType.ToString();
            ValuePrecision = modelVariable.ValuePrecision;
            if (!string.IsNullOrEmpty(modelVariable.ValueEnumerationKey))
                ValueEnumerationKey = modelVariable.ValueEnumerationKey;

            Value = modelVariable.GetValue();
            ValueMaximum = modelVariable.GetValueMaximum();
            ValueMinimum = modelVariable.GetValueMinimum();
        }

        #endregion
    }
}
