using System;

namespace Emlid.WindowsIot.Hardware.Components.Ublox.Ubx
{
    /// <summary>
    /// Defines the receiver ready message. This is sent when the receiver changes from or to backup mode.
    /// </summary>
    [CLSCompliant(false)]
    [Message(MessageClass.Mon, 0x21, MessageType.Output)]
    public class ReceiverStatus : MessageBase
    {
        /// <summary>
        /// Creates an instance with the specified values
        /// </summary>
        public ReceiverStatus()
        {
            Flags = new BitField8();
        }

        /// <summary>
        /// Receiver status flags.
        /// </summary>
        [PayloadIndex(0)]
        public BitField8 Flags { get; set; }

        /// <summary>
        /// Is receiver awake and not in backup mode.
        /// </summary>
        public bool IsAwake
        {
            get { return Flags[0]; }
            private set { Flags[0] = value; }
        }
    }       
}
