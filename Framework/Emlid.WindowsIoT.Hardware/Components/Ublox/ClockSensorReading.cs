
using System;

namespace Emlid.WindowsIot.Hardware.Components.Ublox
{
    /// <summary>
    /// Contains results data from the measurement of the <see cref="Neom8nDevice"/> motion device.
    /// </summary>
    public class ClockSensorReading
    {
        #region Constants

        /// <summary>
        /// Empty value.
        /// </summary>
        public static readonly ClockSensorReading Zero = new ClockSensorReading();

        #endregion

        #region Private Fields

        private double _longitude;
        private double _latitude;
        private double _heightAboveEllipsoid;
        private double _heightAboveSeaLevel;
        private double _horizontalAccuracy;
        private double _herticalAccuracy;

        #endregion

        #region Public Properties

        /// <summary>
        /// GPS time of week of the navigation epoch in ms.
        /// </summary>
        public int TimeMillisOfWeek { get; set; }

        /// <summary>
        /// Longitude in degrees.
        /// </summary>
        public double Longitude
        {
            get { return _longitude / 10000000.0; } // adjusted for scaling
            set { _longitude = value; }
        }

        /// <summary>
        /// Latitude in degrees.
        /// </summary>
        public double Latitude
        {
            get { return _latitude / 10000000.0; } // adjusted for scaling
            set { _latitude = value; }
        }

        /// <summary>
        /// Height above ellipsoid in meters.
        /// </summary>
        public double HeightAboveEllipsoid
        {
            get { return _heightAboveEllipsoid / 1000.0; }
            set { _heightAboveEllipsoid = value; }
        }

        /// <summary>
        /// Height above mean sea level in meters.
        /// </summary>
        public double HeightAboveSeaLevel
        {
            get { return _heightAboveSeaLevel / 1000.0; }
            set { _heightAboveSeaLevel = value; }
        }

        /// <summary>
        /// Horizontal accuracy estimate in millimeter.
        /// </summary>
        public double HorizontalAccuracy
        {
            get { return _horizontalAccuracy / 1000.0; }
            set { _horizontalAccuracy = value; }
        }

        /// <summary>
        /// Vertical accuracy estimate in millimeter.
        /// </summary>
        public double VerticalAccuracy
        {
            get { return _herticalAccuracy / 1000.0; }
            set { _herticalAccuracy = value; }
        }

        #endregion
    }
}
