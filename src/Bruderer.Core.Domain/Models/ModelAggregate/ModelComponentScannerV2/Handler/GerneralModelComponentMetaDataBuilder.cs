﻿using Bruderer.Core.Domain.Models.ModelAggregate.ModelComponentScannerV2.Handler;
using Bruderer.Core.Domain.Models.ModelComponentAggregate.Traversal;
using Bruderer.Core.Domain.Models.ModelComponentContainerAggregate;
using Bruderer.Core.Domain.Models.ModelRPCAggregate;
using Bruderer.Core.Domain.Models.ModelVariableAggregate;
using Bruderer.Core.Domain.Models.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Bruderer.Core.Domain.Models.ModelAggregate.ModelComponentScannerV2.Helpers
{
    public class GerneralModelComponentMetaDataBuilder : RecursiveModelScannerHandler
    {
        private Stack<>
        public override void LeaveModelComponentContainer(PropertyInfo elementProperty, ModelComponentContainer modelComponentContainer)
        {
            
        }

        public override void LeaveModelComponentContainerCollection(PropertyInfo elementProperty, IModelComponentContainerCollection variable)
        {
            throw new NotImplementedException();
        }

        public override void LeaveModelComponentContainerCollectionItem(PropertyInfo elementProperty, ModelComponentContainer modelContainer, int index)
        {
            throw new NotImplementedException();
        }

        public override void LeaveServiceContainer(PropertyInfo elementProperty, ModelComponentContainer serviceContainer)
        {
            throw new NotImplementedException();
        }

        public override void VisitModelComponentContainer(PropertyInfo elementProperty, ModelComponentContainer modelComponentContainer)
        {
            
        }

        public override void VisitModelComponentContainerCollection(PropertyInfo elementProperty, IModelComponentContainerCollection variable)
        {
            throw new NotImplementedException();
        }

        public override void VisitModelComponentContainerCollectionItem(PropertyInfo elementProperty, ModelComponentContainer serviceContainer, int index)
        {
            throw new NotImplementedException();
        }

        public override void VisitModelRPC(PropertyInfo elementProperty, ModelRPCBase rpc)
        {
            throw new NotImplementedException();
        }

        public override void VisitModelVariable(PropertyInfo elementProperty, ModelVariableBase variable)
        {
            throw new NotImplementedException();
        }

        public override void VisitServiceContainer(PropertyInfo elementProperty, ModelComponentContainer serviceContainer)
        {
            throw new NotImplementedException();
        }
    }

    public class ModelScanningModelKeys
    {
        public string ModelPath;
        public string ModelKey;
        public string LocalizationPath;
        public string LocalizationNamespace;
        public string ServiceName;
        public UnitValue EngineeringUnit;
        public ScanningAttributeProps Attributes;
    }
}
