using Bruderer.Core.Domain.Attributes;
using Bruderer.Core.Domain.Attributes.Resolver;
using Bruderer.Core.Domain.Models.ModelAggregate.ModelComponentScannerV2;
using Bruderer.Core.Domain.Models.ModelComponentAggregate;
using Bruderer.Core.Domain.Models.ModelComponentAggregate.Traversal;
using Bruderer.Core.Domain.Models.ModelComponentContainerAggregate;
using Bruderer.Core.Domain.Models.ModelIdentityUserAggregate;
using Bruderer.Core.Domain.Models.ModelRPCAggregate;
using Bruderer.Core.Domain.Models.ModelVariableAggregate;
using Bruderer.Core.Domain.Models.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Bruderer.Core.Domain.Models.ModelAggregate
{
    public class ModelComponentScannerNew: 
    {
        #region fields



        #endregion

        #region ctor

        public ModelComponentScannerNew()
        {

        }


        #endregion
        #region props
        public List<IServiceModelContainer> ServiceModelContainers { get; private set; } = new();
        public List<IRepositoryModelContainer> RepositoryModelContainers { get; private set; } = new();
        public List<IModelComponentContainerCollection> EnumerableModelContainers { get; private set; } = new();

        public ModelComponentContainer ModelComponentTree { get; private set; }
        public List<IModelVariable> Variables { get; private set; } = new List<IModelVariable>();
        public List<IModelRPC> RPCs { get; private set; } = new List<IModelRPC>();

        #endregion

        #region methods
   

        #endregion

        #region methods scanning

        public bool Scan(object modelComponent)
        {
            var modelScannerProps = new ModelScanningProps
            {
                Element = modelComponent
            };

            return Scan(modelScannerProps);
        }

     

    

        #endregion
    }
    public class ScanningWrapperElement
    {
        public object ProxyElement { get; set; }
    }

    public struct ModelScanningProps
    {
        public Guid ModelId;
        public string ModelPath;
        public string ModelKey;
        public string LocalizationPath;
        public string RepositoryPath;
        public string RepositoryTypeName;
        public string RepositoryPartTypeName;
        public Guid RepositoryId;
        public string ServiceName;

        public List<ScanningTwinCAT3AttrubuteProps> TwinCAT3Links;

        public ScanningAttributeProps Attributes;

        public object Element;
        public ModelComponentContainer ParentModelContainer;
    }

    public struct ScanningTwinCAT3AttrubuteProps
    {
        public string Name;
        public string SymbolPath;
        public ModelVariableSamplingRateEnumeration SamplingRate;
        public bool IsIgnored;
    }

    public struct ScanningAttributeProps
    {
        public string LocalizationNamespace;
        public UnitValue? EngineeringUnit;
        public ModelIdentityUserRoleEnumeration VisibilityAuthorization;
        public ModelIdentityUserRoleEnumeration ManipulationAuthorization;
        public List<string> VisibilityConditions;
        public List<string> EditabilityConditions;
        public ModelRepositoryDataFlags? RepositoryDataFlags;
        public bool IsReadOnly;
        public List<string> Filters;
        public List<FilterTrigger> FilterTriggers;
        public int VariableValuePrecision;
    }
}
