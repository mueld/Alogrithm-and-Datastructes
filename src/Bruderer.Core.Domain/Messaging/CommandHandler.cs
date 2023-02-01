using Bruderer.Core.Domain.Messaging.Response;

namespace Bruderer.Core.Domain.Messaging
{
    public abstract class CommandHandler
    {
        #region ctor

        protected CommandHandler ()
        {

        }

        #endregion
        #region props

        public IResponse<bool> ValidationResult { get; set; } = new Response<bool>();

        #endregion
    }
}
