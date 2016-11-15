using System;

namespace Emlid.WindowsIot.Hardware.Components.Ublox.Ubx
{
    /// <summary>
    /// Defines the not-acknowledged UBX protocol message.
    /// </summary>
    [CLSCompliant(false)]
    [Message(MessageClass.Ack, 0x00, MessageType.Output)]
    public class NotAcknowledge : MessageBase
    {
        /// <summary>
        /// Class ID of the not acknowledged message
        /// </summary>
        [PayloadIndex(0)]
        public byte Class { get; set; }

        /// <summary>
        /// Subclass message ID of the not acknowledged message
        /// </summary>
        [PayloadIndex(1)]
        public byte SubClass { get; set; }
    }
}
