using Bruderer.Core.Domain.Models.ModelVariableAggregate;
using System;

namespace Bruderer.Core.Infrastructure.PLC.Connection
{
    public abstract class PLCVariable
    {
        #region ctor

        public PLCVariable(Guid modelId, IModelVariable modelVariable)
        {
            ModelId = modelId;
            ModelVariable = modelVariable;
        }

        #endregion
        #region props

        public Guid ModelId { get; }
        public IModelVariable ModelVariable { get; }

        #endregion
    }
}
