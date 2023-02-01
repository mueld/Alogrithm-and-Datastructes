using Bruderer.Core.Domain.Interfaces;
using MediatR;

namespace Bruderer.Core.Domain.Messaging
{
    public abstract class Event : Message, IEvent, INotification
    {
        #region ctor

        protected Event()
        { }

        #endregion
    }
}
