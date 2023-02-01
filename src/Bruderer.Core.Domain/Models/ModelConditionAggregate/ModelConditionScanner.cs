using Bruderer.Core.Domain.Attributes.Resolver;
using Bruderer.Core.Domain.Models.ModelAggregate;
using Bruderer.Core.Domain.Models.ModelComponentAggregate;
using Bruderer.Core.Domain.Models.ModelComponentContainerAggregate;
using Bruderer.Core.Domain.Models.ModelRPCAggregate;
using Bruderer.Core.Domain.Models.ModelVariableAggregate;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Bruderer.Core.Domain.Models.ModelConditionAggregate
{
    public class ModelConditionScanner : IAdditionalModelComponentScannerFunctionality
    {
        #region fields
        private readonly List<ModelCondition> _ModelConditions;
        private readonly ModelComponentScanner _modelComponentScanner;
        #endregion

        #region ctor

        public ModelConditionScanner(ModelComponentScanner modelScanner, List<ModelCondition> modelConditions)
        {
            _ModelConditions = modelConditions;
            _modelComponentScanner = modelScanner;
        }

        #endregion

        public ModelScanningProps onModelVariable(ModelVariableBase modelVariable, PropertyInfo elementProperty, ModelScanningProps currentModelScanningProps)
        {
            var newVisibilityConditions = new List<string>(currentModelScanningProps.Attributes.VisibilityConditions);
            var newEditabilityConditions = new List<string>(currentModelScanningProps.Attributes.EditabilityConditions);

            setCondition(newVisibilityConditions, AttributeResolver.GetVisibilityConditionsAttribute(elementProperty));
            setCondition(newEditabilityConditions, AttributeResolver.GetEditabilityConditionsAttribute(elementProperty));


            for (var i = 0; i < newVisibilityConditions.Count; i++)
            {
                var condition = newVisibilityConditions[i];

                var foundCondition = _ModelConditions.Find(c => c.ConditionName.Equals(condition));
                if (foundCondition != null)
                {
                    modelVariable.AddVisibilityCondition(foundCondition);
                    foundCondition.AddObserver(modelVariable as IModelConditionObserver);
                }
                else
                {
                    throw new ArgumentException($"ModelCondtion: {condition} on ModelVariable: {modelVariable.ModelLink.Key} could not be found.");
                }

            }

            for (var i = 0; i < newEditabilityConditions.Count; i++)
            {
                var condition = newEditabilityConditions[i];
                var foundCondition = _ModelConditions.Find(c => c.ConditionName.Equals(condition));
                if (foundCondition != null)
                {
                    modelVariable.AddEditabilityCondition(foundCondition);
                    foundCondition.AddObserver(modelVariable as IModelConditionObserver);
                }
                else
                {
                    throw new ArgumentException($"ModelCondtion: {condition} on ModelVariable: {modelVariable.ModelLink.Key} could not be found.");
                }
            }

            return currentModelScanningProps;
        }

        public ModelScanningProps onModelRPC(ModelRPCBase modelRPC, PropertyInfo elementProperty, ModelScanningProps currentModelScanningProps)
        {
            var conditions = AttributeResolver.GetEditabilityConditionsAttribute(elementProperty);

            conditions.ForEach(condition =>
            {
                var foundCondition = _ModelConditions.Find(c => c.ConditionName.Equals(condition));
                if (foundCondition != null)
                {
                    foundCondition.AddObserver(modelRPC as IModelConditionObserver);
                    modelRPC.AddModelCondition(foundCondition);
                }
                else
                {
                    throw new Exception($"ModelCondtion: {condition} on RPC: {modelRPC.ModelLink.Key} could not be found.");
                };
            });
            return currentModelScanningProps;
        }

        public ModelScanningProps onModelContainerCollection(IModelComponentContainerCollection modelComponentContainerCollection, PropertyInfo elementProperty, ModelScanningProps currentModelScanningProps)
        {
            var newVisibilityConditions = new List<string>(currentModelScanningProps.Attributes.VisibilityConditions);
            var newEditabilityConditions = new List<string>(currentModelScanningProps.Attributes.EditabilityConditions);

            setCondition(newVisibilityConditions, AttributeResolver.GetVisibilityConditionsAttribute(elementProperty));
            setCondition(newEditabilityConditions, AttributeResolver.GetEditabilityConditionsAttribute(elementProperty));

            currentModelScanningProps.Attributes.VisibilityConditions = newVisibilityConditions;
            currentModelScanningProps.Attributes.EditabilityConditions = newEditabilityConditions;

            return currentModelScanningProps;
        }

        public ModelScanningProps onModelComponentContainer(ModelComponentContainer modelComponentContainer, PropertyInfo elementProperty, ModelScanningProps currentModelScanningProps)
        {
            var newVisibilityConditions = new List<string>(currentModelScanningProps.Attributes.VisibilityConditions);
            var newEditabilityConditions = new List<string>(currentModelScanningProps.Attributes.EditabilityConditions);

            setCondition(newVisibilityConditions, AttributeResolver.GetVisibilityConditionsAttribute(elementProperty));
            setCondition(newEditabilityConditions, AttributeResolver.GetEditabilityConditionsAttribute(elementProperty));

            addConditions(newVisibilityConditions, modelComponentContainer.VisibilityConditions);
            addConditions(newEditabilityConditions, modelComponentContainer.EditabilityConditions);

            currentModelScanningProps.Attributes.VisibilityConditions = newVisibilityConditions;
            currentModelScanningProps.Attributes.EditabilityConditions = newEditabilityConditions;

            return currentModelScanningProps;
        }

        public bool ScanCompleted()
        {
            for(var i= 0; i< _ModelConditions.Count; i++)
            {
                var condition = _ModelConditions[i];
                foreach(var variable in _modelComponentScanner.Variables)
                {
                    if (condition.TriggerModelLinks.Any(link => link.Equals(variable.ModelLink.Key)))
                    {
                        var modelComponent = variable as ModelComponent;
                        if (modelComponent == null)
                            return false;

                        modelComponent.AddObserver(condition);
                    }
                }

                foreach (var rpc in _modelComponentScanner.RPCs)
                {
                    if (condition.TriggerModelLinks.Any(link => link.Equals(rpc.ModelLink.Key)))
                    {
                        var modelComponent = rpc as ModelComponent;
                        if (modelComponent == null)
                            return false;

                        modelComponent.AddObserver(condition);
                    }
                }

            }
            return true;
        }
    
        private void setCondition(List<string> conditions, List<string> newConditions)
        {
            if(newConditions.Count > 0)
            {
                conditions.Clear();
            }
            foreach(var condition in newConditions)
            {
                if (!conditions.Contains(condition))
                    conditions.Add(condition);
            }
        }

        private void addConditions(List<string> conditions, List<string> newConditions)
        {
            foreach (var condition in newConditions)
            {
                if (!conditions.Contains(condition))
                    conditions.Add(condition);
            }
        }

    }
}
