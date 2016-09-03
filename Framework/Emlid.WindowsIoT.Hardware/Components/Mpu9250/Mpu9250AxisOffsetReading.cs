namespace Emlid.WindowsIot.Hardware.Components.Mpu9250
{
    /// <summary>
    /// Contains results data from the measurement of the <see cref="Mpu9250Device"/> motion device.
    /// </summary>
    public struct Mpu9250AxisOffsetReading
    {
        #region Constants

        /// <summary>
        /// Empty value.
        /// </summary>
        public static readonly Mpu9250AxisOffsetReading Zero = new Mpu9250AxisOffsetReading();

        #endregion

        #region Public Properties

        /// <summary>
        /// Magnetometer radius data.
        /// </summary>
        public short MagRadius { get; set; }

        /// <summary>
        /// Accelerometer radius data.
        /// </summary>
        public short AccelRadius { get; set; }

        /// <summary>
        /// Accelerometer x-axis offset data.
        /// </summary>
        public short AccelXAxisOffset { get; set; }

        /// <summary>
        /// Accelerometer y-axis offset data.
        /// </summary>
        public short AccelYAxisOffset { get; set; }

        /// <summary>
        /// Accelerometer z-axis offset data.
        /// </summary>
        public short AccelZAxisOffset { get; set; }

        /// <summary>
        /// Gyroscope x-axis offset data.
        /// </summary>
        public short GyroXAxisOffset { get; set; }

        /// <summary>
        /// Gyroscope y-axis offset data.
        /// </summary>
        public short GyroYAxisOffset { get; set; }

        /// <summary>
        /// Gyroscope z-axis offset data.
        /// </summary>
        public short GyroZAxisOffset { get; set; }

        /// <summary>
        /// Magnetometer x-axis offset data.
        /// </summary>
        public short MagXAxisOffset { get; set; }

        /// <summary>
        /// Magnetometer y-axis offset data.
        /// </summary>
        public short MagYAxisOffset { get; set; }

        /// <summary>
        /// Magnetometer z-axis offset data.
        /// </summary>
        public short MagZAxisOffset { get; set; }

        #endregion
    }
}
