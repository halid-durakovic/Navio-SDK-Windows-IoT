using System;

namespace Emlid.WindowsIot.Hardware.Components.Ublox.Ubx
{
    /// <summary>
    /// Defines the UBX protocol poll message configuration.
    /// </summary>
    [CLSCompliant(false)]
    [Config]
    [Message(MessageClass.Cfg, 0x01, MessageType.Get | MessageType.Set)]
    public class PollMessageRates : MessageBase
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
        /// Send rate on I/O port 1
        /// </summary>
        [PayloadIndex(2)]
        public byte Port1 { get; set; }

        /// <summary>
        /// Send rate on I/0 port 2
        /// </summary>
        [PayloadIndex(3)]
        public byte Port2 { get; set; }

        /// <summary>
        /// Send rate on I/O port 3
        /// </summary>
        [PayloadIndex(4)]
        public byte Port3 { get; set; }

        /// <summary>
        /// Send rate on I/O port 4
        /// </summary>
        [PayloadIndex(5)]
        public byte Port4 { get; set; }

        /// <summary>
        /// Send rate on I/O port 5
        /// </summary>
        [PayloadIndex(6)]
        public byte Port5 { get; set; }

        /// <summary>
        /// Send rate on I/O port 6
        /// </summary>
        [PayloadIndex(7)]
        public byte Port6 { get; set; }
    }
}
