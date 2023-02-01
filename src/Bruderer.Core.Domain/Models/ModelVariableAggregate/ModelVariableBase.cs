using Bruderer.Core.Domain.Models.ModelComponentAggregate;
using Bruderer.Core.Domain.Models.ModelConditionAggregate;
using Bruderer.Core.Domain.Models.ModelIdentityUserAggregate;
using Bruderer.Core.Domain.Models.ModelUserAggregate;
using Bruderer.Core.Domain.Models.ValueObjects;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bruderer.Core.Domain.Models.ModelVariableAggregate
{
    public abstract class ModelVariableBase : ModelComponent, IModelConditionObserver
    {
        #region fields
        protected List<IModelCondition> _VisibilityConditions = new();
        protected List<IModelCondition> _EditabilityConditions = new();
        #endregion

        #region ctor
        public ModelVariableBase() {}
        #endregion

        #region props

        [NotMapped]
        public LocalizableValue Display { get; set; } = new();
        [NotMapped]
        public LocalizableValue Description { get; set; } = new();

        [NotMapped]
        public ModelIdentityUserRoleEnumeration VisibilityAuthorization { get; set; } = ModelIdentityUserRoleEnumeration.Undefined;
        [NotMapped]
        public ModelIdentityUserRoleEnumeration ManipulationAuthorization { get; set; } = ModelIdentityUserRoleEnumeration.Undefined;
        
        [NotMapped]
        public bool IsRepositoryClient { get; set; } = false;
        public string RepositoryTypeName { get; set; } = string.Empty;
        public string RepositoryPartTypeName { get; set; } = string.Empty;
        public Guid RepositoryId { get; set; } = Guid.Empty;
        public string RepositoryLink { get; set; } = string.Empty;

        [NotMapped]
        public UnitValue? EngineeringUnit { get; set; } = null;
        [NotMapped]
        public bool IsReadOnly { get; set; } = false;
        [NotMapped]
        public bool IsVisible { get; set; } = true;
        [NotMapped]
        public bool IsInvalid { get; set; } = false;
        public int ScanningIndex { get; set; } = -1;
        [NotMapped]
        public abstract Type ValueType { get; protected set; }
        [NotMapped]
        public int? ValuePrecision { get; set; } = null;
        [NotMapped]
        public abstract string? ValueEnumerationKey { get; }

        #endregion
        #region methods

        public abstract object? GetValue();
        public abstract void SetValue(object? newValue);
        public abstract object? GetValueMaximum();
        public abstract void SetValueMaximum(object? newValue);
        public abstract object? GetValueMinimum();
        public abstract void SetValueMinimum(object? newValue);

        public IModelVariableDEO GetDEO(bool includeReference = true)
        {
            return new ModelVariableDEO(this as IModelVariable, includeReference);
        }

        public void AddVisibilityCondition(IModelCondition condition)
        {
            if (!_VisibilityConditions.Contains(condition))
            {
                _VisibilityConditions.Add(condition);
            }
        }

        public void RemoveVisibilityCondition(IModelCondition condition)
        {
            _VisibilityConditions.Remove(condition);
        }

        public void AddEditabilityCondition(IModelCondition condition)
        {
            if (!_EditabilityConditions.Contains(condition))
            {
                _EditabilityConditions.Add(condition);
            }
        }

        public void RemoveEditabilityCondition(IModelCondition condition)
        {
            _EditabilityConditions.Remove(condition);
        }

        public virtual void Take(IModelVariable sourceVariable, bool takeMetadata = true, bool takeValue = true, bool onlyRespositoryData = false)
        {
            // OFI -> Check for auto mapping solutions as https://github.com/AutoMapper/AutoMapper

            if (onlyRespositoryData)
            {
                IsRepositoryClient = true;
                RepositoryTypeName = sourceVariable.RepositoryTypeName;
                RepositoryLink = sourceVariable.RepositoryLink;

                ScanningIndex = sourceVariable.ScanningIndex;

                SetValue(sourceVariable.GetValue());
                SetValueMaximum(sourceVariable.GetValueMaximum());
                SetValueMinimum(sourceVariable.GetValueMinimum());

                return;
            }

            if (takeMetadata)
            {
                // Set props by reference
                if (sourceVariable.ParentModelContainer != null)
                    ParentModelContainer = sourceVariable.ParentModelContainer;

                // Set props by atomic copy
                Id = sourceVariable.Id;

                ModelLink.Path = sourceVariable.ModelLink.Path;
                ModelLink.Name = sourceVariable.ModelLink.Name;

                ServiceName = sourceVariable.ServiceName;

                TwinCAT3Links.Clear();
                sourceVariable.TwinCAT3Links.ForEach(linkData => TwinCAT3Links.Add(new ModelTwinCAT3Link(linkData)));
                OPCUALinks.Clear();
                sourceVariable.OPCUALinks.ForEach(linkData => OPCUALinks.Add(new ModelOPCUALink(linkData)));

                Display.KeyNamespace = sourceVariable.Display.KeyNamespace;
                Display.Value = sourceVariable.Display.Value;
                Display.Link.Path = sourceVariable.Display.Link.Path;
                Display.Link.Name = sourceVariable.Display.Link.Name;
                Display.DynamicValueDictionary = new Dictionary<string, string>(sourceVariable.Display.DynamicValueDictionary);

                Description.KeyNamespace = sourceVariable.Description.KeyNamespace;
                Description.Value = sourceVariable.Description.Value;
                Description.Link.Path = sourceVariable.Description.Link.Path;
                Description.Link.Name = sourceVariable.Description.Link.Name;
                Description.DynamicValueDictionary = new Dictionary<string, string>(sourceVariable.Description.DynamicValueDictionary);

                VisibilityAuthorization = sourceVariable.VisibilityAuthorization;
                ManipulationAuthorization = sourceVariable.ManipulationAuthorization;

                IsRepositoryClient = sourceVariable.IsRepositoryClient;
                IsPersistent = sourceVariable.IsPersistent;
                IsInvalid = sourceVariable.IsInvalid;
                RepositoryTypeName = sourceVariable.RepositoryTypeName;
                RepositoryPartTypeName = sourceVariable.RepositoryPartTypeName;
                RepositoryId = sourceVariable.RepositoryId;
                ScanningIndex = sourceVariable.ScanningIndex;
                RepositoryLink = sourceVariable.RepositoryLink;

                Version = sourceVariable.Version;

                if (EngineeringUnit != null &&
                    sourceVariable.EngineeringUnit != null)
                {
                    EngineeringUnit.Description = sourceVariable.EngineeringUnit.Description;
                    EngineeringUnit.DisplayName = sourceVariable.EngineeringUnit.DisplayName;
                    EngineeringUnit.UNECECode = sourceVariable.EngineeringUnit.UNECECode;
                    EngineeringUnit.UnitID = sourceVariable.EngineeringUnit.UnitID;
                }

                IsReadOnly = sourceVariable.IsReadOnly;
                Filters = new List<string>(sourceVariable.Filters);

                ValuePrecision = sourceVariable.ValuePrecision;
            }

            if (takeValue)
            {
                SetValue(sourceVariable.GetValue());
                SetValueMaximum(sourceVariable.GetValueMaximum());
                SetValueMinimum(sourceVariable.GetValueMinimum());
            }
        }
        public virtual void Take(IModelVariableDEO modelVariableDEO, bool takeId = false, bool takeModelLink = false)
        {
            // OFI -> Check for auto mapping solutions as https://github.com/AutoMapper/AutoMapper

            if (takeId)
                Id = modelVariableDEO.ModelContext.Id;

            if (takeModelLink)
            {
                ModelLink.Path = modelVariableDEO.ModelContext.ModelLink.Path;
                ModelLink.Name = modelVariableDEO.ModelContext.ModelLink.Name;
            }

            SetValue(modelVariableDEO.Payload.Value);
            SetValueMaximum(modelVariableDEO.Payload.ValueMaximum);
            SetValueMinimum(modelVariableDEO.Payload.ValueMinimum);
        }

        public override bool ShouldSerializeId()
        {
            return false;
        }
        public bool ShouldSerializeDisplay()
        {
            return false;
        }
        public bool ShouldSerializeDescription()
        {
            return false;
        }
        public bool ShouldSerializeVisibilityAuthorization()
        {
            return false;
        }
        public bool ShouldSerializeManipulationAuthorization()
        {
            return false;
        }
        public bool ShouldSerializeIsRepositoryClient()
        {
            return false;
        }
        public bool ShouldSerializeRepositoryModelContainer()
        {
            return false;
        }
        public bool ShouldSerializeEngineeringUnit()
        {
            return false;
        }
        public bool ShouldSerializeIsReadOnly()
        {
            return false;
        }
        public bool ShouldSerializeIsInvalid()
        {
            return false;
        }
        public bool ShouldSerializeScanningIndex()
        {
            return false;
        }
        public bool ShouldSerializeValueType()
        {
            return false;
        }
        public bool ShouldSerializeValuePrecision()
        {
            return false;
        }
        public bool ShouldSerializeValueEnumerationKey()
        {
            return false;
        }


        #endregion

        #region IModelConditionObserver
        public virtual void ConditionChanged(IModelCondition condition)
        {
            if(_VisibilityConditions.Count > 0)
            {
                IsVisible = _VisibilityConditions.TrueForAll(c => c.IsFulfilled);
            }
            if (_EditabilityConditions.Count > 0)
            {
                IsReadOnly = !_EditabilityConditions.TrueForAll(c => c.IsFulfilled) && IsPersistent;
            }
        }
        #endregion
    }
}
