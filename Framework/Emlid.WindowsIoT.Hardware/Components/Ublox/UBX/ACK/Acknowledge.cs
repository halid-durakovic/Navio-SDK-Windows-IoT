using System;

namespace Emlid.WindowsIot.Hardware.Components.Ublox.Ubx
{
    /// <summary>
    /// Defines the acknowledged UBX protocol message
    /// </summary>
    [CLSCompliant(false)]
    [Message(MessageClass.Ack, 0x01, MessageType.Output)]
    public class Acknowledge : MessageBase
    {
        /// <summary>
        /// Class identifier of the acknowledged message
        /// </summary>
        [PayloadIndex(0)]
        public byte Class { get; set; }

        /// <summary>
        /// Subclass message identifier of the acknowledged message
        /// </summary>
        [PayloadIndex(1)]
        public byte SubClass { get; set; }
    }
}
