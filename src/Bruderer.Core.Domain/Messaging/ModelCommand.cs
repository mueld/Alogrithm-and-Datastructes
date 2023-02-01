using Bruderer.Core.Domain.Interfaces;
using System;

namespace Bruderer.Core.Domain.Messaging
{
    public abstract class ModelCommand : Command, IModelCommand
    {
        #region ctor

        protected ModelCommand(Guid modelId)
        {
            ModelId = modelId;
        }

        #endregion
        #region props

        public Guid ModelId { get; protected set; } = Guid.Empty;
        public string ModelName { get; set; } = string.Empty;

        #endregion
    }

    public abstract class ModelCommand<T> : Command<T>, IModelCommand
    {
        #region ctor

        protected ModelCommand(Guid modelId)
        {
            ModelId = modelId;
        }

        #endregion
        #region props

        public Guid ModelId { get; protected set; } = Guid.Empty;
        public string ModelName { get; set; } = string.Empty;

        #endregion
    }
}
