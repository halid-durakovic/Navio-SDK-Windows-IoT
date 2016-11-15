using System;

namespace Emlid.WindowsIot.Hardware.Components.Ublox.Ubx
{
    /// <summary>
    /// Defines the receiver navigation status UBX protocol message.
    /// </summary>
    [CLSCompliant(false)]
    [Message(MessageClass.Mon, 0x04, MessageType.PollRequest)]
    public class ReceiverSoftwareRequest : MessageBase
    {
        /// <summary>
        /// Zero-terminated software version string.
        /// </summary>
        [PayloadIndex(0)]
        public byte Reserved { get; private set; } 
    }
}
