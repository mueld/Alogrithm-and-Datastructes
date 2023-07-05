using Bruderer.Core.Domain.Attributes.Resolver;
using Bruderer.Core.Domain.Constants;
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

namespace Bruderer.Core.Domain.Models.ModelAggregate.ModelComponentScannerV2.Handler
{
    public class RepositoriesModelHandler : RecursiveModelScannerHandler
    {
        private Stack<ModelRepositoryPropeties> _stack = new Stack<ModelRepositoryPropeties>();

        public RepositoriesModelHandler()
        {
            CurrentProperties = new();
        }
        public RepositoriesModelHandler(ModelRepositoryPropeties seeding)
        {
            CurrentProperties = seeding;
        }


        public ModelRepositoryPropeties CurrentProperties { get; private set; }
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

        public override void VisitModelComponentContainer(PropertyInfo elementProperty, ModelComponentContainer modelComponentContainer)
        {
            var modelContainerType = modelComponentContainer.GetType();
            
            var newPropeties = new ModelRepositoryPropeties(CurrentProperties);
            _stack.Push(CurrentProperties);
            CurrentProperties = newPropeties;

            CurrentProperties.RepositoryDataFlags =UpdateRepositoryDataFlags(elementProperty);

            if (modelContainerType.GetInterfaces().Contains(typeof(IRepositoryModelContainer)))
            {
                CurrentProperties.RepositoryTypeName = modelComponentContainer.GetType().Name;
                CurrentProperties.RepositoryPartTypeName = CurrentProperties.RepositoryTypeName
                CurrentProperties.RepositoryPath = CurrentProperties.RepositoryTypeName;

                var repositoryModelContainer = modelComponentContainer as IRepositoryModelContainer;
                
                if (repositoryModelContainer.RepositoryCreationPolicy == RepositoryCreationPolicyEnumeration.Partial &&
                    string.IsNullOrEmpty(CurrentProperties.RepositoryTypeName))
                    throw new InvalidOperationException($"Repository container [{modelContainerType.Name}] has a partial creation policy and no repository name defined. You must override the {nameof(modelContainer.RepositoryTypeName)} propety when the interface {nameof(IRepositoryModelContainer)} is implemented and the creation plolicy is set to {RepositoryCreationPolicyEnumeration.Partial}.");


            }
            if (!string.IsNullOrEmpty(CurrentProperties.RepositoryTypeName))
            {
                CurrentProperties.RepositoryPath = BuildRepositoryPath(modelComponentContainer, elementProperty);
            }
            modelComponentContainer.RepositoryId = CurrentProperties.RepositoryId;
            modelComponentContainer.RepositoryLink = CurrentProperties.RepositoryPath;
            modelComponentContainer.RepositoryPartTypeName = CurrentProperties.RepositoryPartTypeName;
            modelComponentContainer.IsRepositoryClient = true;



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

        private static ModelRepositoryDataFlags UpdateRepositoryDataFlags(PropertyInfo elementProperty)
        {
            var newRepositoryDataFlags = ModelRepositoryDataFlags.None; ;

            // Resolve attribute
            var attributeRepositoryData = AttributeResolver.GetRepositoryDataAttribute(elementProperty);
            if (attributeRepositoryData != null)
            {
                var attributeRepositoryDataResolved = (ModelRepositoryDataFlags)attributeRepositoryData;
                if (attributeRepositoryDataResolved.HasFlag(ModelRepositoryDataFlags.None))
                    newRepositoryDataFlags = ModelRepositoryDataFlags.None;
                else
                    newRepositoryDataFlags = attributeRepositoryDataResolved;
            }

            return newRepositoryDataFlags;
        }

        private string BuildRepositoryPath(
        ModelComponentContainer modelContainer,
        PropertyInfo elementProperty,
        string seperator = StringConstants.Separator)
        {
            if (string.IsNullOrEmpty(CurrentProperties.RepositoryPath))
                return modelContainer.GetType().Name;
            else
                return (CurrentProperties.RepositoryPath + seperator + elementProperty.Name);
        }
    }

    public class ModelRepositoryPropeties
    {
        public ModelRepositoryPropeties() { }
        public ModelRepositoryPropeties(ModelRepositoryPropeties seeding)
        {
            RepositoryPath = seeding.RepositoryPath;
            RepositoryTypeName = seeding.RepositoryTypeName;
            RepositoryPartTypeName = seeding.RepositoryPartTypeName;
            RepositoryId= seeding.RepositoryId;
        }

            public string RepositoryPath = string.Empty;
            public string RepositoryTypeName = string.Empty;
            public string RepositoryPartTypeName = string.Empty;
            public ModelRepositoryDataFlags RepositoryDataFlags = ModelRepositoryDataFlags.None;
            public Guid RepositoryId = Guid.Empty;
    }
}
