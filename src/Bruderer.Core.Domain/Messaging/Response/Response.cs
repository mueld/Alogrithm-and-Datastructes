using Bruderer.Core.Domain.Models.ValueObjects;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Bruderer.Core.Domain.Messaging.Response
{
    public class Response : IResponse
    {
        #region props

        public ResponseResultEnumeration Result { get; set; } = ResponseResultEnumeration.Unknown;
        public List<IResponseMessage> MessageStack { get; set; } = new List<IResponseMessage>();
        public string LogText { get; set; } = string.Empty;

        #endregion
        #region methods

        public void AddMessage(Exception ex)
        {
            var message = new ResponseMessage
            {
                Context = ex.Source
            };

            message.Display.Value = ex.Message;
            message.LogText = ex.ToString();

            try
            {
                // Get stack trace for the exception with source file information
                var st = new StackTrace(ex, true);

                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);

                if (frame != null)
                {
                    message.LineNumber = frame.GetFileLineNumber();
                    message.Caller = frame.GetFileName();
                }
            }
            catch (Exception){ }

            MessageStack.Add(message);
        }

        public void AddMessage(IResponseMessage message)
        {
            if (message != null)
                MessageStack.Add(message);
        }

        public void AddMessage(LocalizableValue localizedMessage, string context, string logMessage = null, [CallerLineNumber] int lineNumber = -1, [CallerMemberName] string caller = null)
        {
            var message = new ResponseMessage
            {
                Display = localizedMessage,
                Context = context
            };

            if (logMessage != null)
                message.LogText = logMessage;

            if (lineNumber > 0)
                message.LineNumber = lineNumber;

            if (caller != null)
                message.Caller = caller;

            MessageStack.Add(message);
        }

        public void LogInfo(string message)
        {
            LogText = message;
            Result = ResponseResultEnumeration.Info;
        }

        public void LogSuccess()
        {
            Result = ResponseResultEnumeration.Success;
        }

        public void LogSuccess(string message)
        {
            LogText = message;
            Result = ResponseResultEnumeration.Success;
        }

        public void LogWarning(string message)
        {
            LogText = message;
            Result = ResponseResultEnumeration.Warning;
        }

        public void LogError(string message)
        {
            LogText = message;
            Result = ResponseResultEnumeration.Error;
        }

        public void LogDebug(string message)
        {
            LogText = message;
            Result = ResponseResultEnumeration.Debug;
        }

        public bool IsSuccesfully()
        {
            return (Result == ResponseResultEnumeration.Success);
        }

        #endregion
    }

    public class Response<T> : Response, IResponse<T>
    {
        #region ctor

        public Response()
        {
            Payload = default;
        }

        public Response(T payload)
        {
            Payload = payload;
        }

        public Response(T payload, ResponseResultEnumeration result)
        {
            Result = result;
            Payload = payload;
        }

        #endregion
        #region props

        public T Payload { get; set; } = default;

        #endregion
        #region methods

        public void LogSuccess(T payload)
        {
            Payload = payload;
            Result = ResponseResultEnumeration.Success;
        }

        public void LogError(T payload, string message)
        {
            Payload = payload;
            LogText = message;
            Result = ResponseResultEnumeration.Error;
        }

        #endregion
    }
}
