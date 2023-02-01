using Bruderer.Core.Domain.Constants;
using System;

namespace Bruderer.Core.Infrastructure.PLC.TwinCAT3.Exceptions
{
    [Serializable]
    public class TC3ADSDisconnectException : InvalidOperationException
    {
        public const string DefaultMessage = "Error on disconnect an existing TwinCAT 3 connection.";

        public TC3ADSDisconnectException()
            : base(DefaultMessage)
        {
            HResult = HResults.E_TC3_ENV_ADS_DISCONNECT;
        }

        public TC3ADSDisconnectException(string addressSpecifier)
            : base(DefaultMessage)
        {
            HResult = HResults.E_TC3_ENV_ADS_DISCONNECT;
            InjectData(addressSpecifier);
        }

        public TC3ADSDisconnectException(Exception inner)
            : base(DefaultMessage, inner)
        {
            HResult = HResults.E_TC3_ENV_ADS_DISCONNECT;
        }

        public TC3ADSDisconnectException(string addressSpecifier, Exception inner)
            : base(DefaultMessage, inner)
        {
            HResult = HResults.E_TC3_ENV_ADS_DISCONNECT;
            InjectData(addressSpecifier);
        }

        private void InjectData(string addressSpecifier)
        {
            Data.Add("TC3_AddressSpecifier", addressSpecifier);
        }
    }
}
