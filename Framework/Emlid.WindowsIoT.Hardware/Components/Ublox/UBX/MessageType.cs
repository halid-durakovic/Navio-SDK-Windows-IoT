using System;

namespace Emlid.WindowsIot.Hardware.Components.Ublox.Ubx
{
    /// <summary>
    /// Message type
    /// </summary>
    [Flags]
    public enum MessageType
    {
        /// <summary>
        /// Input message request
        /// </summary>
        Input = 0x0,

        /// <summary>
        /// Output from processing an input message
        /// </summary>
        Output = 0x01,

        /// <summary>
        /// Poll request message
        /// </summary>
        PollRequest = 0x02,

        /// <summary>
        /// Output from processing a poll request message
        /// </summary>
        Polled = 0x04,

        /// <summary>
        /// Periodic message
        /// </summary>
        Periodic = 0x08,

        /// <summary>
        /// Command message
        /// </summary>
        Command = 0x16,

        /// <summary>
        /// Get message
        /// </summary>
        Get = 0x32,

        /// <summary>
        /// Set message
        /// </summary>
        Set = 0x64
    }
}
