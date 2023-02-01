using Bruderer.Core.Domain.Models.ValueObjects;

namespace Bruderer.Core.Domain.Messaging.Response
{
    public interface IResponseMessage
    {
        LocalizableValue Display { get; set; }
        string Context { get; set; }
        string LogText { get; set; }
        int LineNumber { get; set; }
        string Caller { get; set; }
    }
}
