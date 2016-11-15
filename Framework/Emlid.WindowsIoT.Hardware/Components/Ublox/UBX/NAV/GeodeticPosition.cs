using System;

namespace Emlid.WindowsIot.Hardware.Components.Ublox.Ubx
{
    /// <summary>
    /// Defines the geodetic position solution UBX protocol message.
    /// </summary>
    [CLSCompliant(false)]
    [Message(MessageClass.Nav, 0x02, MessageType.Polled | MessageType.Output )]
    public class GeodeticPosition : MessageBase
    {
        /// <summary>
        /// GPS time of week of the navigation epoch in ms.
        /// </summary>
        [PayloadIndex(0)]
        public uint TimeMillisOfWeek { get; private set; }

        /// <summary>
        /// Longitude in degree.
        /// </summary>
        [PayloadIndex(1)]
        public int Longitude { get; set; }

        /// <summary>
        /// Latitude in degree.
        /// </summary>
        [PayloadIndex(2)]
        public int Latitude { get; set; }

        /// <summary>
        /// Height above ellipsoid in millimeter.
        /// </summary>
        [PayloadIndex(3)]
        public int HeightAboveEllipsoid { get; private set; }

        /// <summary>
        /// Height above mean sea level in millimeter.
        /// </summary>
        [PayloadIndex(4)]
        public int HeightAboveSeaLevel { get; private set; }

        /// <summary>
        /// Horizontal accuracy estimate in millimeter.
        /// </summary>
        [PayloadIndex(5)]
        public uint HorizontalAccuracy { get; private set; }
        
        /// <summary>
        /// Vertical accuracy estimate in millimeter.
        /// </summary>
        [PayloadIndex(6)]
        public uint VerticalAccuracy { get; private set; }
    }
}
