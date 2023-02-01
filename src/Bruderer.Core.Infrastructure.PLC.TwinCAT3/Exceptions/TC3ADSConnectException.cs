using Bruderer.Core.Domain.Constants;
using System;

namespace Bruderer.Core.Infrastructure.PLC.TwinCAT3.Exceptions
{
    [Serializable]
    public class TC3ADSConnectException : InvalidOperationException
    {
        public const string DefaultMessage = "Error on establish TwinCAT 3 connection.";

        public TC3ADSConnectException()
            : base(DefaultMessage)
        {
            HResult = HResults.E_TC3_ENV_ADS_CONNECT;
        }

        public TC3ADSConnectException(string addressSpecifier)
            : base(DefaultMessage)
        {
            HResult = HResults.E_TC3_ENV_ADS_CONNECT;
            InjectData(addressSpecifier);
        }

        public TC3ADSConnectException(Exception inner)
            : base(DefaultMessage, inner)
        {
            HResult = HResults.E_TC3_ENV_ADS_CONNECT;
        }

        public TC3ADSConnectException(string addressSpecifier, Exception inner)
            : base(DefaultMessage, inner)
        {
            HResult = HResults.E_TC3_ENV_ADS_CONNECT;
            InjectData(addressSpecifier);
        }

        private void InjectData(string addressSpecifier)
        {
            Data.Add("TC3_AddressSpecifier", addressSpecifier);
        }
    }
}
