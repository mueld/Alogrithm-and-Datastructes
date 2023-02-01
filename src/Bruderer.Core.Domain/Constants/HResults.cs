namespace Bruderer.Core.Domain.Constants
{
    public class HResults
    {
        public const int S_OK = 0x00000000;
        public const long E_ABORT = 0x80004004;
        public const long E_FAIL = 0x80004005;
        public const long E_UNEXPECTED = 0x8000FFFF;

        public const int E_CLI_BAD_PARSING = 0x10000;
        public const int E_CLI_BAD_VERSION_REQUEST = 0x10001;
        public const int E_CLI_BAD_HELP_REQUEST = 0x10002;

        public const int E_TC3_ENV_ADS_CONNECT = 0x11000;
        public const int E_TC3_ENV_ADS_DISCONNECT = 0x11001;
        public const int E_TC3_ENV_ADS_SET_STATE = 0x11020;
        public const int E_TC3_ENV_ADS_STATE_BAD_ARGUMENT = 0x11021;

        public const int E_TC3_ENV_DTE_SETUP = 0x11100;
        public const int E_TC3_ENV_SOLUTION_SETUP = 0x11101;
        public const int E_TC3_ENV_SYSTEMMANAGER = 0x11102;
        public const int E_TC3_ENV_BOOTPROJECT = 0x11103;
        public const int E_TC3_ENV_SOLUTION_BUILD = 0x11104;
        public const int E_TC3_ENV_CONFIGURATION_ACTIVATE = 0x11105;
        public const int E_TC3_ENV_AUTORUN = 0x11106;

        public const int E_DATA_MODELLOC_BAD_ASSEMBLY = 0x12000;
        public const int E_DATA_MODELLOC_BAD_MODELOBJ = 0x12001;
        public const int E_DATA_MODELLOC_BAD_SERIALIZATION = 0x12002;

        public const int E_DATA_ZIP_BAD_MODE = 0x12100;
    }
}
