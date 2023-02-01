using Bruderer.Core.Domain.Interfaces;
using Bruderer.Core.Domain.Messaging;
using Bruderer.Core.Domain.Messaging.Response;
using Bruderer.Core.Domain.Models.ModelRPCAggregate;
using Bruderer.Core.Domain.Models.ModelRPCAggregate.Arguments;
using Bruderer.Core.Domain.Models.ModelRPCAggregate.Response;
using System;
using System.Collections.Generic;

namespace Bruderer.Core.Domain.Commands
{
    public class ModelRpcCommand : ModelCommand<ModelRPCResponse>, IModelRpcCommand
    {
        #region ctor

        public ModelRpcCommand(Guid modelId, Guid rpcId, IList<object> inputArguments)
            : base (modelId)
        {
            RpcId = rpcId;
            InputArguments = inputArguments;
        }

        public ModelRpcCommand(Guid modelId, IModelRPC rpcMethod)
            : base(modelId)
        {
            RpcId = rpcMethod.Id;
            InputArguments = rpcMethod.GetInputArguments();
        }

        #endregion
        #region props

        public Guid RpcId { get; protected set; }
        public IList<object> InputArguments { get; protected set; }

        #endregion
        #region methods

        public override bool IsValid()
        {
            ValidationResult.Payload = new ModelRPCResponse();
            ValidationResult.Result = ResponseResultEnumeration.Success;
            return ValidationResult.Result == ResponseResultEnumeration.Success;
        }

        #endregion
    }

    public class ModelRpcCommand<T> : ModelCommand<RPCResponse<T>>, IModelRpcCommand
        where T : ModelRPCOutputArgumentContainer
    {
        #region ctor

        public ModelRpcCommand(Guid modelId, Guid rpcId, IList<object> inputArguments)
            : base(modelId)
        {
            RpcId = rpcId;
            InputArguments = inputArguments;
        }

        public ModelRpcCommand(Guid modelId, IModelRPC rpcMethod)
            : base(modelId)
        {
            RpcId = rpcMethod.Id;
            InputArguments = rpcMethod.GetInputArguments();
        }

        #endregion
        #region props

        public Guid RpcId { get; protected set; }
        public IList<object> InputArguments { get; protected set; }

        #endregion
        #region methods

        public override bool IsValid()
        {
            ValidationResult.Payload = new RPCResponse<T>();
            ValidationResult.Result = ResponseResultEnumeration.Success;
            return ValidationResult.Result == ResponseResultEnumeration.Success;
        }

        #endregion
    }
}
