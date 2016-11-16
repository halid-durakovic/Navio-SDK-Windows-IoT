using System;

namespace Emlid.WindowsIot.Hardware.Components.Ublox.Ubx
{
    /// <summary>
    /// Defines the UBX protocol poll message rate configuration.
    /// </summary>  
    [Configuration]
    [Acknowledged]
    [CLSCompliant(false)]
    [Message(MessageClass.Cfg, 0x01, MessageType.Get | MessageType.Set)]
    public class PollingMessageRate : MessageBase
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

        /// <summary>
        /// Send rate on current Port
        /// </summary>
        [PayloadIndex(2)]
        public byte Rate { get; set; }
    }
}
