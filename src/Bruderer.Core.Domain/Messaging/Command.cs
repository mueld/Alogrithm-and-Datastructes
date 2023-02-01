using Bruderer.Core.Domain.Interfaces;
using Bruderer.Core.Domain.Messaging.Response;
using MediatR;

namespace Bruderer.Core.Domain.Messaging
{
    public abstract class Command : Message, ICommand, IRequest<IResponse<bool>>
    {
        #region ctor

        protected Command()
        { }

        #endregion
        #region props

        public IResponse<bool> ValidationResult { get; set; } = new Response<bool>(false);

        #endregion
        #region methods

        public virtual bool IsValid() { return ValidationResult.Result == ResponseResultEnumeration.Success; }

        #endregion
    }

    public abstract class Command<T> : Message, ICommand, IRequest<IResponse<T>>
    {
        #region ctor

        protected Command()
        { }

        #endregion
        #region props

        public IResponse<T> ValidationResult { get; set; } = new Response<T>(default);

        #endregion
        #region methods

        public virtual bool IsValid() { return ValidationResult.Result == ResponseResultEnumeration.Success; }

        #endregion
    }
}
