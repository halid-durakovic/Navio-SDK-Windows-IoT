using System;
using System.Collections.Generic;

namespace Emlid.WindowsIot.Hardware.Components.Ublox.Ubx
{
    /// <summary>
    /// Defines the space vehicle info UBX protocol message. 
    /// </summary>
    [CLSCompliant(false)]
    [Message(MessageClass.Nav, 0x30, MessageType.Input | MessageType.Output)]
    public class SpaceVehicleInfo : MessageBase
    {
        /// <summary>
        /// GPS time of week of the navigation epoch in milliseconds
        /// </summary>
        [PayloadIndex(0)]
        public uint TimeMillisOfWeek { get; set; }

        /// <summary>
        /// Number of channels
        /// </summary>
        [PayloadIndex(1)]
        public byte ChannelCount { get; set; }

        /// <summary>
        /// Global flags bitmask
        /// </summary>
        [PayloadIndex(1)]
        public byte GlobalFlags { get; private set; }

        /// <summary>
        /// Reserved
        /// </summary>
        [PayloadIndex(3)]
        public ushort Reserved1 { get; private set; }

        /// <summary>
        /// Repeat block for <see cref="ChannelCount"/>
        /// </summary>
        [PayloadIndex(4)]
        public IEnumerable<SpaceVehicleChannelItem> ChannelList { get; private set; }

        /// <summary>
        /// Repeat block for <see cref="ChannelList"/>
        /// </summary>
        [PayloadBlock]
        public struct SpaceVehicleChannelItem
        {
            /// <summary>
            /// Channel number, 255 for SVs not assigned to a channel
            /// </summary>
            [PayloadIndex(0)]
            public byte ChannelNumber { get; private set; }

            /// <summary>
            /// Satellite Id, see Satellite numbering for assignment
            /// </summary>
            [PayloadIndex(1)]
            public byte SatteliteID { get; private set; }

            /// <summary>
            /// Bitmask Flags
            /// </summary>
            [PayloadIndex(2)]
            public byte Flags { get; private set; }

            /// <summary>
            /// Bitfield
            /// </summary>
            [PayloadIndex(3)]
            public byte Quality { get; private set; }

            /// <summary>
            /// Carrier to noise ratio (signal strength) in dBHz
            /// </summary>
            [PayloadIndex(4)]
            public byte SignalStrength { get; private set; }

            /// <summary>
            /// Elevation in integer degrees
            /// </summary>
            [PayloadIndex(5)]
            public sbyte Elevation { get; private set; }

            /// <summary>
            /// Aximuth in integer degrees
            /// </summary>
            [PayloadIndex(6)]
            public short Azimuth { get; private set; }

            /// <summary>
            /// Pseudo range residual in centimeters
            /// </summary>
            [PayloadIndex(7)]
            public int PseudoRangeResidual { get; private set; }
        }
    }
}
