using Bruderer.Core.Domain.Models.ModelVariableAggregate;
using System;
using System.Collections.Generic;

namespace Bruderer.Core.Domain.Exceptions
{
    [Serializable]
    public class PushModelVariableDataException : InvalidOperationException
    {
        public const string DefaultMessage = "Error pushing variable data.";

        public PushModelVariableDataException(string contextName, IEnumerable<IModelVariableDEO> modelVariableDataList)
            : base(DefaultMessage)
        {
            InjectData(contextName, modelVariableDataList);
        }

        public PushModelVariableDataException(string contextName, IEnumerable<IModelVariable> modelVariableDataList)
            : base(DefaultMessage)
        {
            InjectData(contextName, modelVariableDataList);
        }

        public PushModelVariableDataException(string contextName, IEnumerable<IModelVariableDEO> modelVariableDataList, Exception inner)
            : base(DefaultMessage, inner)
        {
            InjectData(contextName, modelVariableDataList);
        }

        public PushModelVariableDataException(string contextName, IEnumerable<IModelVariable> modelVariableDataList, Exception inner)
            : base(DefaultMessage, inner)
        {
            InjectData(contextName, modelVariableDataList);
        }

        private void InjectData(string contextName, IEnumerable<IModelVariableDEO> modelVariableDataList)
        {
            Data.Add("ContextName", contextName);
            var dataContextNULLCount = 0;
            var dataPayloadNULLCount = 0;

            foreach (var modelVariableData in modelVariableDataList)
            {
                if (modelVariableData.ModelContext == null)
                {
                    dataContextNULLCount++;
                    continue;
                }
                    
                if (modelVariableData.ModelContext.ModelLink == null)
                {
                    dataContextNULLCount++;
                    continue;
                }

                if (modelVariableData.Payload == null)
                {
                    dataPayloadNULLCount++;
                    continue;
                }

                if (modelVariableData.Payload.Value == null)
                {
                    dataPayloadNULLCount++;
                    continue;
                }

                Data.Add(modelVariableData.ModelContext.ModelLink.Key, modelVariableData.Payload.Value);
            }

            Data.Add("DataContext_NULL_Count", dataContextNULLCount);
            Data.Add("DataPayload_NULL_Count", dataPayloadNULLCount);
        }

        private void InjectData(string contextName, IEnumerable<IModelVariable> modelVariableDataList)
        {
            Data.Add("ContextName", contextName);
            var variableNULLCount = 0;
            var variableValueNULLCount = 0;

            foreach (var modelVariableData in modelVariableDataList)
            {
                if (modelVariableData == null)
                {
                    variableNULLCount++;
                    continue;
                }

                if (modelVariableData.GetValue() == null)
                {
                    variableValueNULLCount++;
                    continue;
                }

                Data.Add(modelVariableData.ModelLink.Key, modelVariableData.GetValue());
            }

            Data.Add("Variable_NULL_Count", variableNULLCount);
            Data.Add("VariableValue_NULL_Count", variableValueNULLCount);
        }
    }
}
