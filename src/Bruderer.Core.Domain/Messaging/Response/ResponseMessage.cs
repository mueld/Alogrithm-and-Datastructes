using Bruderer.Core.Domain.Models.ValueObjects;

namespace Bruderer.Core.Domain.Messaging.Response
{
    public class ResponseMessage : IResponseMessage
    {
        #region ctor

        public ResponseMessage(){}

        #endregion
        #region props

        public LocalizableValue Display { get; set; } = new LocalizableValue();
        public string Context { get; set; } = string.Empty;
        public string LogText { get; set; } = string.Empty;
        public int LineNumber { get; set; } = -1;
        public string Caller { get; set; } = string.Empty;

        #endregion
    }
}
