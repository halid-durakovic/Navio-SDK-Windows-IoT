using System;

namespace Emlid.WindowsIot.Hardware.Components.Ublox.Ubx
{
    /// <summary>
    /// Defines the dilution of precision UBX protocol message.
    /// </summary>
    [CLSCompliant(false)]
    [Message(MessageClass.Nav, 0x04, MessageType.Periodic | MessageType.Polled )]
    public class DilutionPrecision : MessageBase
    {
        /// <summary>
        /// GPS time of week of the navigation epoch in ms.
        /// </summary>
        [PayloadIndex(0)]
        public uint TimeMillisOfWeek { get; private set; }

        /// <summary>
        /// Geometric dilution of precision
        /// </summary>
        [PayloadIndex(1)]
        public ushort Geometric { get; set; }

        /// <summary>
        /// Position dilution of precision
        /// </summary>
        [PayloadIndex(2)]
        public ushort Position { get; set; }

        /// <summary>
        /// Time dilution of precision
        /// </summary>
        [PayloadIndex(3)]
        public ushort Time { get; set; }

        /// <summary>
        /// Vertical dilution of precision
        /// </summary>
        [PayloadIndex(4)]
        public ushort Vertical { get; set; }

        /// <summary>
        /// Horizontal dilution of precision
        /// </summary>
        [PayloadIndex(5)]
        public ushort Horizontal { get; set; }

        /// <summary>
        /// Northing dilution of precision
        /// </summary>
        [PayloadIndex(6)]
        public ushort Northing { get; set; }

        /// <summary>
        /// Easting dilution of precision
        /// </summary>
        [PayloadIndex(7)]
        public ushort Easting { get; set; }
    }
}
