using Bruderer.Core.Infrastructure.PLC.Connection;
using Bruderer.Core.Domain.Models.ModelVariableAggregate;
using TwinCAT.TypeSystem;
using System;

namespace Bruderer.Core.Infrastructure.PLC.TwinCAT3.Connection
{
    public class TC3Variable : PLCVariable
    {
        #region ctor

        public TC3Variable(Guid modelId, IModelVariable modelVariable)
            : base(modelId, modelVariable)
        {

        }

        #endregion
        #region props

        public ISymbol Symbol { get; set; } = null;

        #endregion
    }
}
