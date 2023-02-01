using Bruderer.Core.Domain.Models.ValueObjects;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Bruderer.Core.Domain.Messaging.Response
{
    public interface IResponse
    {
        ResponseResultEnumeration Result { get; set; }
        List<IResponseMessage> MessageStack { get; set; }
        string LogText { get; set; }

        void AddMessage(Exception ex);
        void AddMessage(IResponseMessage message);
        void AddMessage(LocalizableValue localizedMessage, string context, string logMessage = null, [CallerLineNumber] int lineNumber = -1, [CallerMemberName] string caller = null);

        void LogInfo(string message);
        void LogSuccess(string message);
        void LogWarning(string message);
        void LogError(string message);
        void LogDebug(string message);
        bool IsSuccesfully();
    }


    public interface IResponse<T> : IResponse
    {
        T Payload { get; set; }
    }
}
