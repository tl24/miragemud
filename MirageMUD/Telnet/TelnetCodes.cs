namespace Mirage.Telnet
{
    public enum TelnetCodes : byte
    {
        /*
         * Main Codes
         */
        /// <summary>
        /// Sent to suggest that the opposite end should support an option
        /// </summary>
        WILL = 251,

        /// <summary>
        /// Sent to suggest that the opposite end should not support an option
        /// </summary>
        WONT = 252,
        /// <summary>
        /// Sent to suggest that the sender can support an option, or
        /// in confirmation to a WILL request
        /// </summary>
        DO = 253,
        /// <summary>
        /// Sent to suggest that the sender can't or won't support an option, or
        /// in confirmation to a WILL/WONT request
        /// </summary>
        DONT = 254,

        /// <summary>
        /// Any sequence should start with IAC (Interpret as command).
        /// To send IAC as data, send 2 in sequence as an escape
        /// </summary>
        IAC = 255,

        /// <summary>
        /// Subnegotiation begin, preceded by IAC
        /// </summary>
        SB = 250,

        /// <summary>
        /// Subnegotiation end, preceded by IAC
        /// </summary>
        SE = 240,
        /*
         * Option Codes
         */
        ECHO = 1,
        /// <summary>
        /// Supress Go-Ahead
        /// </summary>
        SUPRESS_GA = 3,
        TERMINAL_TYPE = 24,
        WINDOW_SIZE = 31,
        TERMINAL_SPEED = 32,
        ENV_VARIABLES = 36,
        NEW_ENVIRONMENT = 39
    }
}