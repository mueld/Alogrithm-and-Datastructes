using Bruderer.Core.Infrastructure.PLC.TwinCAT3.Exceptions;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using TwinCAT;
using TwinCAT.Ads;

namespace Bruderer.Core.Infrastructure.PLC.TwinCAT3.Connection
{
    /// <summary>
    /// Reperesents a TwinCAT 3 session to interact with the specified ADS endpoint.
    /// </summary>
    public class TC3Session : AdsSession
    {
        #region fields

        private readonly ILogger<TC3Session> _Logger = null;

        #endregion
        #region ctor

        public TC3Session(string amsNetId, int portNumber)
            : base(new AmsAddress(amsNetId, portNumber), SessionSettings.Default)
        {

        }

        public TC3Session(string amsNetId, int portNumber, ILoggerFactory loggerFactory)
            : base(new AmsAddress(amsNetId, portNumber), SessionSettings.Default)
        {
            _Logger = loggerFactory.CreateLogger<TC3Session>();
        }

        public TC3Session(TC3ConnectionProps connectionProps, ILoggerFactory loggerFactory)
            : base(connectionProps.AmsAddress, SessionSettings.Default)
        {
            _Logger = loggerFactory.CreateLogger<TC3Session>();
        }

        #endregion
        #region props

        public Version TwinCATVersion { get; private set; } = new Version();
        public AdsConnection AdsConnection { get; private set; } = default;
        public AdsState AdsState { get; private set; } = default;
        public short DeviceState { get; private set; } = default;
        public bool IsActive
        {
            get
            {
                return (AdsConnection != null) &&
                    ConnectionState == ConnectionState.Connected &&
                    AdsState == AdsState.Run &&
                    DeviceState == 0;
            }
        }

        #endregion
        #region methods

        /// <summary>
        /// Connects to a TwinCAT 3 ADS object. The connection parameters are passed through the constructor.
        /// </summary>
        /// <exception cref="TC3ADSConnectException">
        /// Thrown when connection to a TwinCAT 3 ADS object failed.
        /// </exception>
        public Task<bool> Connect(CancellationToken cancellationToken)
        {
            try
            {
                // Connect ADS session
                AdsConnection = (AdsConnection)Connect();
                if (AdsConnection == null)
                    throw new TC3ADSConnectException(AddressSpecifier,
                        new ArgumentException($"Cannot resolve [{typeof(AdsConnection).Name}] object.", paramName: nameof(AdsConnection)));

                // Register ADS connection event handlers
                AdsConnection.ConnectionStateChanged += AdsConnection_ConnectionStateChanged;
                //AdsConnection.AdsStateChanged += AdsConnection_AdsStateChanged;
                AdsConnection.RouterStateChanged += AdsConnection_RouterStateChanged;

                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                AdsState = AdsState.Invalid;
                DeviceState = 0;
                AdsConnection = null;

                throw new TC3ADSConnectException(AddressSpecifier, ex);
            }
        }

        /// <summary>
        /// Disconnetcs from a TwinCAT 3 ADS object.
        /// </summary>
        /// <exception cref="TC3ADSDisconnectException">
        /// Thrown when disconnection from a TwinCAT 3 ADS object failed.
        /// </exception>
        public Task<bool> Disconnect(CancellationToken cancellationToken)
        {
            try
            {
                if (AdsConnection != null)
                {
                    AdsConnection.Disconnect();
                    AdsConnection.ConnectionStateChanged -= AdsConnection_ConnectionStateChanged;
                    //AdsConnection.AdsStateChanged -= AdsConnection_AdsStateChanged;
                    AdsConnection.RouterStateChanged -= AdsConnection_RouterStateChanged;
                }

                AdsState = AdsState.Invalid;
                DeviceState = 0;
                AdsConnection = null;

                return Task.FromResult(Disconnect());
            }
            catch (Exception ex)
            {
                AdsState = AdsState.Invalid;
                DeviceState = 0;
                AdsConnection = null;

                throw new TC3ADSDisconnectException(AddressSpecifier, ex);
            }
        }

        /// <summary>
        /// Reads state informations from the TwinCAT 3 ADS connection. This method can be used for cyclic evaluation.
        /// </summary>
        public async Task CheckConnection(CancellationToken cancellationToken)
        {
            if (AdsConnection == null)
                return;

            try
            {
                // Read the device informations
                ResultDeviceInfo resultDeviceInfo = await AdsConnection.ReadDeviceInfoAsync(cancellationToken);
                if (resultDeviceInfo.Succeeded)
                {
                    var adsVersion = resultDeviceInfo.DeviceInfo.Version;
                    TwinCATVersion = new Version($"{adsVersion.Version}.{adsVersion.Revision}.{adsVersion.Build}");
                }
                else
                {
                    if (_Logger != null)
                        _Logger.LogTrace($"TwinCAT 3 connection for [{AddressSpecifier}] could not read device informations.");
                }

                // Read the states
                ResultReadDeviceState resultReadDeviceState = await AdsConnection.ReadStateAsync(cancellationToken);
                if (resultReadDeviceState.Succeeded)
                {
                    AdsState = resultReadDeviceState.State.AdsState;
                    DeviceState = resultReadDeviceState.State.DeviceState;
                }
                else
                {
                    AdsState = AdsState.Invalid;
                    DeviceState = 0;

                    if (_Logger != null)
                        _Logger.LogTrace($"TwinCAT 3 connection for [{AddressSpecifier}] could not read device states.");
                }
            }
            catch (Exception ex)
            {
                _Logger.LogTrace($"Unexpected error on check TwinCAT 3 connection for [{AddressSpecifier}]. {ex.Message}");
            }
        }

        #endregion
        #region events

        private void AdsConnection_ConnectionStateChanged(object sender, ConnectionStateChangedEventArgs e)
        {
            var exceptionMessage = "";
            if (e.Exception != null)
                exceptionMessage = e.Exception.Message;

            if (e.NewState == ConnectionState.Connected)
            {
                if (_Logger != null)
                    _Logger.LogInformation($"TwinCAT 3 connection state has changed from [{e.OldState}] to [{e.NewState}] because of {e.Reason}. {exceptionMessage}");
            }
            else
            {
                if (_Logger != null)
                    _Logger.LogWarning($"TwinCAT 3 connection state has changed from [{e.OldState}] to [{e.NewState}] because of {e.Reason}. {exceptionMessage}");
            }
        }

        private void AdsConnection_RouterStateChanged(object sender, AmsRouterNotificationEventArgs e)
        {
            if (_Logger != null)
                _Logger.LogWarning($"TwinCAT 3 AMS router state has changed to {e.State}");
        }

        private void AdsConnection_AdsStateChanged(object sender, AdsStateChangedEventArgs e)
        {
            AdsState = e.State.AdsState;
            DeviceState = e.State.DeviceState;

            if (DeviceState == 0)
            {
                if (_Logger != null)
                    _Logger.LogInformation($"TwinCAT 3 device is ready.");
            }
            else
            {
                if (_Logger != null)
                    _Logger.LogWarning($"TwinCAT 3 device not ready. State has changed to {e.State.AdsState}.");
            }

            if (AdsState == AdsState.Run)
            {
                if (_Logger != null)
                    _Logger.LogInformation($"TwinCAT 3 ADS state has changed to {e.State.AdsState}.");
            }
            else
            {
                if (_Logger != null)
                    _Logger.LogWarning($"TwinCAT 3 ADS state has changed to {e.State.AdsState}.");
            }
        }

        #endregion
    }
}
