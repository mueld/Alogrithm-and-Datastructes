using Bruderer.Core.Domain.Models.ModelRPCAggregate.Arguments;

#nullable enable

namespace Bruderer.Core.Domain.Models.ModelRPCAggregate.Response
{
    public class ModelRPCResponse
    {
        #region ctor

        public ModelRPCResponse(){}

        public ModelRPCResponse(ModelRPCResponseResultEnumeration result, object? returnValue)
        {
            Result = result;
            ReturnValue = returnValue;
        }

        #endregion
        #region props

        public ModelRPCResponseResultEnumeration Result { get; set; } = ModelRPCResponseResultEnumeration.Unknown;
        public object? ReturnValue { get; set; } = null;

        #endregion
    }

    public class RPCResponse<T>
        where T : ModelRPCOutputArgumentContainer
    {
        #region ctor

        public RPCResponse(){}

        public RPCResponse(ModelRPCResponseResultEnumeration result, T returnValues)
        {
            Result = result;
            ReturnValue = returnValues;
        }

        #endregion
        #region props

        public ModelRPCResponseResultEnumeration Result { get; set; } = ModelRPCResponseResultEnumeration.Unknown;
        public T? ReturnValue { get; set; } = null;

        #endregion
    }
}
