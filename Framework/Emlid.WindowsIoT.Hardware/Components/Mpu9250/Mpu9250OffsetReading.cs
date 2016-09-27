namespace Emlid.WindowsIot.Hardware.Components.Mpu9250
{
    /// <summary>
    /// Contains results data from the measurement of the <see cref="Mpu9250Device"/> motion device.
    /// </summary>
    public struct Mpu9250OffsetReading
    {
        #region Constants

        /// <summary>
        /// Empty value.
        /// </summary>
        public static readonly Mpu9250OffsetReading Zero = new Mpu9250OffsetReading();

        #endregion

        #region Public Properties

        /// <summary>
        /// Accelerometer x-axis offset data.
        /// </summary>
        public double AccelXAxisOffset { get; set; }

        /// <summary>
        /// Accelerometer y-axis offset data.
        /// </summary>
        public double AccelYAxisOffset { get; set; }

        /// <summary>
        /// Accelerometer z-axis offset data.
        /// </summary>
        public double AccelZAxisOffset { get; set; }

        /// <summary>
        /// Gyroscope x-axis offset data.
        /// </summary>
        public double GyroXAxisOffset { get; set; }

        /// <summary>
        /// Gyroscope y-axis offset data.
        /// </summary>
        public double GyroYAxisOffset { get; set; }

        /// <summary>
        /// Gyroscope z-axis offset data.
        /// </summary>
        public double GyroZAxisOffset { get; set; }

        /// <summary>
        /// Magnetometer x-axis offset data.
        /// </summary>
        public double MagXAxisOffset { get; set; }

        /// <summary>
        /// Magnetometer y-axis offset data.
        /// </summary>
        public double MagYAxisOffset { get; set; }

        /// <summary>
        /// Magnetometer z-axis offset data.
        /// </summary>
        public double MagZAxisOffset { get; set; }

        /// <summary>
        /// Magnetometer x-axis sensitivity offset data.
        /// </summary>
        public double MagXAxisSensitivity { get; set; }

        /// <summary>
        /// Magnetometer y-axis sensitivity offset data.
        /// </summary>
        public double MagYAxisSensitivity { get; set; }

        /// <summary>
        /// Magnetometer z-axis sensitivity offset data.
        /// </summary>
        public double MagZAxisSensitivity { get; set; }

        #endregion
    }
}
