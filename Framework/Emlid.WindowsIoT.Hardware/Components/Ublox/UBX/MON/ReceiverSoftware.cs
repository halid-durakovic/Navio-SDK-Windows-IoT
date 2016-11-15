using System;

namespace Emlid.WindowsIot.Hardware.Components.Ublox.Ubx
{
    /// <summary>
    /// Defines the receiver navigation status UBX protocol message.
    /// </summary>
    [CLSCompliant(false)]
    [Message(MessageClass.Mon, 0x04, MessageType.Polled)]
    public class ReceiverSoftware : MessageBase
    {
        /// <summary>
        /// Creates an instance with the specified values
        /// </summary>
        public ReceiverSoftware()
        {
            SoftwareVersion = new byte[30];
            HardwareVersion = new byte[10];
            Extentions = new byte[150]; // 5 extentions  TODO: Check length
        }

        /// <summary>
        /// Zero-terminated software version string.
        /// </summary>
        [PayloadIndex(0, 30)]
        public byte[] SoftwareVersion { get; private set; }

        /// <summary>
        /// Zero-terminated hardware version string.
        /// </summary>
        [PayloadIndex(1, 10)]
        public byte[] HardwareVersion { get; private set; }

        /// <summary>
        /// Extended software information strings. A series of zero-terminated strings.
        /// </summary>
        [PayloadIndex(2, 150)]
        public byte[] Extentions { get; private set; }
    }
}
