using Bruderer.Core.Domain.Models.ModelAggregate.ModelComponentScannerV2.Handler;
using Bruderer.Core.Domain.Models.ModelComponentAggregate.Traversal;
using Bruderer.Core.Domain.Models.ModelComponentContainerAggregate;
using Bruderer.Core.Domain.Models.ModelRPCAggregate;
using Bruderer.Core.Domain.Models.ModelVariableAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Bruderer.Core.Domain.Models.ModelAggregate.ModelComponentScannerV2.Helpers
{
    public class ModelComponentRelationshipResolver : RecursiveModelScannerHandler
    {

        private ModelComponentContainer _ParentModelContainer;
        private Stack<ModelComponentContainer> _Stack;

        public ModelComponentRelationshipResolver(ModelComponentContainer RootContainer)
        {
            ModelComponentTree = RootContainer;
            _ParentModelContainer = RootContainer;
            _Stack = new();
            _Stack.Push(ModelComponentTree);
        }

        public ModelComponentRelationshipResolver()
        {
            ModelComponentTree = new();
            _ParentModelContainer = new();
            _Stack = new();
            _Stack.Push(ModelComponentTree);
        }

       

        public override void VisitModelComponentContainer(PropertyInfo elementProperty, ModelComponentContainer modelComponentContainer)
        {
            modelComponentContainer.ParentModelContainer = _ParentModelContainer;
            _ParentModelContainer.ModelContainers.Add(modelComponentContainer);

            _ParentModelContainer = modelComponentContainer;
            _Stack.Push(modelComponentContainer);
        }

        public override void VisitModelComponentContainerCollection(PropertyInfo elementProperty, IModelComponentContainerCollection collection)
        {
            
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

        public override void LeaveModelComponentContainer(PropertyInfo elementProperty, ModelComponentContainer modelComponentContainer)
        {
            throw new NotImplementedException();
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


        public ModelComponentContainer ModelComponentTree { get; private set; } 

        public List<IModelRPC> RPCs { get; private set; } = new List<IModelRPC>();

        public List<IModelVariable> Variables { get; private set; } = new List<IModelVariable>();

        public List<IServiceModelContainer> ServiceModelContainers { get; private set; } = new();

        public List<IRepositoryModelContainer> RepositoryModelContainers { get; private set; } = new();

        public List<IModelComponentContainerCollection> EnumerableModelContainers { get; private set; } = new();


    }
   
}
