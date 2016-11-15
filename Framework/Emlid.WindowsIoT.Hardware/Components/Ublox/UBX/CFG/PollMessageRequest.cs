using System;

namespace Emlid.WindowsIot.Hardware.Components.Ublox.Ubx
{
    /// <summary>
    /// Defines the UBX protocol poll message configuration.
    /// </summary>
    [CLSCompliant(false)]
    [Config]
    [Message(MessageClass.Cfg, 0x01, MessageType.PollRequest)]
    public class PollMessageRequest : MessageBase
    {
        /// <summary>
        /// Class identifier of the poll message
        /// </summary>
        [PayloadIndex(0)]
        public byte Class { get; set; }

        /// <summary>
        /// Subclass message identifier of the poll message
        /// </summary>
        [PayloadIndex(1)]
        public byte SubClass { get; set; }
    }
}
