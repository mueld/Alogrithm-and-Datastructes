using Bruderer.Core.Domain.Models.ModelComponentContainerAggregate;
using Bruderer.Core.Domain.Models.ModelConditionAggregate;
using Bruderer.Core.Domain.Models.ModelRPCAggregate;
using Bruderer.Core.Domain.Models.ModelVariableAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Bruderer.Core.Domain.Models.ModelAggregate
{
    public  interface IAdditionalModelComponentScannerFunctionality
    {
        public ModelScanningProps onModelVariable(ModelVariableBase modelVariable, PropertyInfo elementProperty, ModelScanningProps currentModelScanningProps);
        public ModelScanningProps onModelRPC(ModelRPCBase modelRPC, PropertyInfo elementProperty, ModelScanningProps currentModelScanningProps);
        public ModelScanningProps onModelContainerCollection(IModelComponentContainerCollection modelComponentContainerCollection, PropertyInfo elementProperty, ModelScanningProps currentModelScanningProps);
        public ModelScanningProps onModelComponentContainer(ModelComponentContainer modelComponentContainer, PropertyInfo elementProperty, ModelScanningProps currentModelScanningProps);

        public bool ScanCompleted();
       
    }
}
