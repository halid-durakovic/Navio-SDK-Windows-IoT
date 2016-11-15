using System;

namespace Emlid.WindowsIot.Hardware.Components.Ublox.Ubx
{
    /// <summary>
    /// Defines the receiver navigation status UBX protocol message.
    /// </summary>
    [CLSCompliant(false)]
    [Message(MessageClass.Nav, 0x03, MessageType.Periodic | MessageType.Polled)]
    public class NavigationStatus : MessageBase
    {
        /// <summary>
        /// Creates an instance with the specified values
        /// </summary>
        public NavigationStatus()
        {
            Fix = FixStatus.NoFix;
            Flags = new BitField8();
            Flags2 = new BitField8();
            FixStat = new BitField8();
        }

        /// <summary>
        /// GPS time of week of the navigation epoch in ms.
        /// </summary>
        [PayloadIndex(0)]
        public uint TimeMillisOfWeek { get; private set; }

        /// <summary>
        /// GPS fix status
        /// </summary>
        [PayloadIndex(1)]
        public FixStatus Fix { get; private set; }

        /// <summary>
        /// Navigation status flags
        /// </summary>
        [PayloadIndex(2)]
        public BitField8 Flags { get; private set; }

        /// <summary>
        /// Navigation fix status information
        /// </summary>
        [PayloadIndex(3)]
        public BitField8 FixStat { get; private set; }

        /// <summary>
        /// Further navigation informaion about navagtion output 
        /// </summary>
        [PayloadIndex(4)]
        public BitField8 Flags2 { get; private set; }

        /// <summary>
        /// Time to first fix in millisecond time tag
        /// </summary>
        [PayloadIndex(5)]
        public uint TimeToFirstFix { get; private set; }

        /// <summary>
        /// Milliseconds since startup/reset
        /// </summary>
        [PayloadIndex(6)]
        public uint TimeSinceStartUp { get; private set; }

        /// <summary>
        /// Is time of week valid
        /// </summary>
        public bool IsTowSet
        {
            get { return Flags[3]; }
            private set { Flags[3] = value; }
        }

        /// <summary>
        /// Is week number valid
        /// </summary>
        public bool IsWknSet
        {
            get { return Flags[2]; }
            private set { Flags[2] = value; }
        }

        /// <summary>
        /// Is DGPS used
        /// </summary>
        public bool IsDiffSoln
        {
            get { return Flags[1]; }
            private set { Flags[1] = value; }
        }

        /// <summary>
        /// Is DGPS used
        /// </summary>
        public bool IsGpsFixOk
        {
            get { return Flags[0]; }
            private set { Flags[0] = value; }
        }

        /// <summary>
        /// GPS fix type 
        /// </summary>
        public enum FixStatus : byte
        {
            /// <summary>
            /// No fix
            /// </summary>
            NoFix = 0x00,

            /// <summary>
            /// Dead reckoning only
            /// </summary>
            DeadReckoning = 0x01,

            /// <summary>
            /// 2D fix
            /// </summary>
            Fix2D = 0x02,

            /// <summary>
            /// 3D fix
            /// </summary>
            Fix3D = 0x03,

            /// <summary>
            /// GPS plus dead reckoning combinded
            /// </summary>
            GPSDeadReckoning = 0x04,

            /// <summary>
            /// Time only fix
            /// </summary>
            TimeOnly = 0x05
        }
    }
}
