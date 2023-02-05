using Bruderer.Core.Domain.Models.ModelComponentAggregate.Traversal;
using Bruderer.Core.Domain.Models.ModelComponentContainerAggregate;
using Bruderer.Core.Domain.Models.ModelRPCAggregate;
using Bruderer.Core.Domain.Models.ModelVariableAggregate;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Bruderer.Core.Domain.Models.ModelAggregate.ModelComponentScannerV2.Helpers
{
    public class ModelComponetValueDistribution : Visitor
    {
        private Queue<IModelVariable> _ModelVariablesToDistribute;
        private int _ProccessingIndex;
        private IModelVariable _NextModelVariable;
        private bool _AllVariablesAreDistributed;


        public ModelComponetValueDistribution(List<IModelVariable> modelVariablesToDistribute)
        {
            modelVariablesToDistribute.Sort((IModelVariable x, IModelVariable y) => { return x.ScanningIndex.CompareTo(y.ScanningIndex); });


            _ModelVariablesToDistribute = new Queue<IModelVariable>(modelVariablesToDistribute);
            _NextModelVariable= _ModelVariablesToDistribute.Dequeue();
        }
        public override void VisitModelComponentContainer(PropertyInfo elementProperty, ModelComponentContainer modelComponentContainer)
        {
            
        }

        public override void VisitModelComponentContainerCollection(PropertyInfo elementProperty, IModelComponentContainerCollection variable)
        {
            
        }

        public override void VisitModelComponentContainerCollectionItem(PropertyInfo elementProperty, ModelComponentContainer serviceContainer, int index)
        {
            
        }

        public override void VisitModelRPC(PropertyInfo elementProperty, ModelRPCBase rpc)
        {
            
        }

        public override void VisitModelVariable(PropertyInfo elementProperty, ModelVariableBase variable)
        {
            _ProccessingIndex++;

            if (_AllVariablesAreDistributed)
                return;

            if(_ProccessingIndex == _NextModelVariable.ScanningIndex)
            {
                variable.Take(_NextModelVariable);
                if(_ModelVariablesToDistribute.Count > 0)
                {
                    _NextModelVariable = _ModelVariablesToDistribute.Dequeue();
                }
                else
                {
                    _AllVariablesAreDistributed = true;
                }
                
            }
        }

        public override void VisitServiceContainer(PropertyInfo elementProperty, ModelComponentContainer serviceContainer)
        {
            
        }
    }

}
