using Bruderer.Core.Domain.Models.ModelConditionAggregate;
using Bruderer.Core.Domain.Models.ModelRPCAggregate.Arguments;
using System;
using System.Collections.Generic;

namespace Bruderer.Core.Domain.Models.ModelRPCAggregate
{
    public class ModelRPC<TIn, TOut> : ModelRPCBase, IModelRPC
       where TIn : ModelRPCInputArgumentContainer
       where TOut : ModelRPCOutputArgumentContainer
    {
        #region fields

        private TIn _RPCInputArgumentContainer = Activator.CreateInstance(typeof(TIn)) as TIn;
        private TOut _RPCOutputArgumentContainer = Activator.CreateInstance(typeof(TOut)) as TOut;

        #endregion
        #region ctor

        public ModelRPC() { }

        #endregion
        #region props

        public override Type InputArgumentType { get { return typeof(TIn); } }

        public override Type OutputArgumentType { get { return typeof(TOut); } }

        #endregion
        #region methods

        public TOut GetOutputArgumentContainer(object outputArgument)
        {
            if (outputArgument == null)
                return default;

            IList<object> outputArguments = new List<object>();
            var outputArgumentType = outputArgument.GetType();
            if (outputArgumentType.IsArray)
            {
                var outputArgumentArray = outputArgument as object[];
                outputArguments = new List<object>(outputArgumentArray);
            }
            else
            {
                outputArguments.Add(outputArgument);
            }

            return ModelRPCArgumentResolver.GetOutputArgumentContainer<TOut>(outputArguments);
        }

        public TOut GetOutputArgumentContainer(IList<object> outputArguments)
        {
            return ModelRPCArgumentResolver.GetOutputArgumentContainer<TOut>(outputArguments);
        }

        #endregion

        #region IRPCMethod

        public T GetInputArgument<T>()
            where T : ModelRPCInputArgumentContainer
        {
            if (typeof(TIn) == typeof(T))
                return _RPCInputArgumentContainer as T;

            return default;
        }

        public bool SetInputArgument<T>(T inputArgumentContainer)
            where T : ModelRPCInputArgumentContainer
        {
            if (typeof(TIn) == typeof(T))
            {
                _RPCInputArgumentContainer = inputArgumentContainer as TIn;
                return true;
            }

            return false;
        }

        public IList<object> GetInputArguments()
        {
            return ModelRPCArgumentResolver.GetInputArguments(_RPCInputArgumentContainer);
        }

        public IList<ModelRPCArgument> GetInputArguments<T>()
            where T : ModelRPCArgument
        {
            return ModelRPCArgumentResolver.GetArguments(_RPCInputArgumentContainer);
        }

        public IList<object> GetOutputArguments()
        {
            return ModelRPCArgumentResolver.GetOutputArguments(_RPCOutputArgumentContainer);
        }

        public IList<ModelRPCArgument> GetOutputArguments<T>()
            where T : ModelRPCArgument
        {
            return ModelRPCArgumentResolver.GetArguments(_RPCOutputArgumentContainer);
        }

        public T GetOutputArgumentContainer<T>(object outputArgument)
            where T : ModelRPCOutputArgumentContainer
        {
            if (outputArgument == null)
                return default;

            IList<object> outputArguments = new List<object>();
            var outputArgumentType = outputArgument.GetType();
            if (outputArgumentType.IsArray)
            {
                var outputArgumentArray = outputArgument as object[];
                outputArguments = new List<object>(outputArgumentArray);
            }
            else
            {
                outputArguments.Add(outputArgument);
            }

            return ModelRPCArgumentResolver.GetOutputArgumentContainer<T>(outputArguments);
        }

        public T GetOutputArgumentContainer<T>(IList<object> outputArguments)
            where T : ModelRPCOutputArgumentContainer
        {
            return ModelRPCArgumentResolver.GetOutputArgumentContainer<T>(outputArguments);
        }

       

        #endregion
    }
}
