using System;
using Emlid.WindowsIot.Hardware.Filters;

namespace Emlid.WindowsIot.Hardware.Components.Mpu9250
{
    /// <summary>
    /// Contains results data from the measurement of the <see cref="Mpu9250Device"/> motion device.
    /// </summary>
    public class Mpu9250SensorReading
    {
        #region Constants

        /// <summary>
        /// Empty value.
        /// </summary>
        public static readonly Mpu9250SensorReading Zero = new Mpu9250SensorReading();

        #endregion

        #region Private Fields

        /// <summary>
        /// Madgwick AHRS algorithm object.
        /// </summary>
        private MadgwickAHRS _ahrs = new MadgwickAHRS(1f / 256f, 0.1f);
 
        #endregion

        #region Public Properties

        /// <summary>
        /// Accelerometer x-axis in meters/second^2. 
        /// </summary>
        public float AccelXAxis { get; set; }

        /// <summary>
        /// Accelerometer y-axis in meters/second^2. 
        /// </summary>
        public float AccelYAxis { get; set; }

        /// <summary>
        /// Accelerometer z-axis in meters/second^2.
        /// </summary>
        public float AccelZAxis { get; set; }

        /// <summary>
        /// Gyroscope x-axis in degrees per second. 
        /// </summary>
        public float GyroXAxis { get; set; }

        /// <summary>
        /// Gyroscope y-axis in degrees per second. 
        /// </summary>
        public float GyroYAxis { get; set; }

        /// <summary>
        /// Gyroscope z-axis in degrees per second. 
        /// </summary>
        public float GyroZAxis { get; set; }

        /// <summary>
        /// Magnetometer x-axis in micro-teslas.
        /// </summary>
        public float MagXAxis { get; set; }

        /// <summary>
        /// Magnetometer y-axis in micro-teslas.
        /// </summary>
        public float MagYAxis { get; set; }

        /// <summary>
        /// Magnetometer z-axis in micro-teslas.
        /// </summary>
        public float MagZAxis { get; set; }

        /// <summary>
        /// MPU-9250 die temperature in celsius.
        /// </summary>
        public float Temperature { get; set; }

        /// <summary>
        /// Quaternion w-axis in degrees.
        /// </summary>
        public float QuaternionWAxis { get; set; }

        /// <summary>
        /// Quaternion x-axis in quaternion units.
        /// </summary>
        public float QuaternionXAxis { get; set; }

        /// <summary>
        /// Quaternion y-axis in quaternion units.
        /// </summary>
        public float QuaternionYAxis { get; set; }

        /// <summary>
        /// Quaternion z-axis in quaternion units.
        /// </summary>
        public float QuaternionZAxis { get; set; }
        /// <summary>
        /// Euler heading angles in degrees.
        /// </summary>
        public float EulerYaw { get; set; }

        /// <summary>
        /// Euler roll angles in degrees. 
        /// </summary>
        public float EulerRoll { get; set; }

        /// <summary>
        /// Euler pitch angles in degrees. 
        /// </summary>
        public float EulerPitch { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Update sensor readings using Madgwick AHRS algorithm.
        /// </summary>
        public void Update()
        {
            _ahrs.Update(Deg2rad(GyroXAxis), Deg2rad(GyroYAxis), Deg2rad(GyroZAxis), AccelXAxis, AccelYAxis, AccelZAxis, MagXAxis, MagYAxis, MagZAxis);
 
            QuaternionWAxis = _ahrs.Quaternion[0];
            QuaternionXAxis = _ahrs.Quaternion[1];
            QuaternionYAxis = _ahrs.Quaternion[2];
            QuaternionZAxis = _ahrs.Quaternion[3];
            EulerRoll = _ahrs.Euler[0];
            EulerPitch = _ahrs.Euler[1];
            EulerYaw = _ahrs.Euler[2];
        }

        /// <summary>
        /// Converts degrees to radians.
        /// </summary>
        /// <param name="degrees">
        /// Angular quantity in degrees.
        /// </param>
        /// <returns>
        /// Angular quantity in radians.
        /// </returns>
        static float Deg2rad(float degrees)
        {
            return (float)(Math.PI / 180) * degrees;
        }

        /// <summary>
        /// Returns a string representation of the current contents,
        /// e.g. "Pressure: 1013.43155085426 Temperature:36.3892484283447".
        /// </summary>
        public override string ToString()
        {
            //return string.Format(CultureInfo.CurrentCulture,
            //    Resources.Strings.MS5611MeasurementStringFormat, Pressure, Temperature);

            return string.Format("Accel: x={0} y={1} z={2} Gyro: x={3} y={4} z={5} Mag: x={6} y={7} z={8} Quat: w={9} x={10} y={11} z={12} Euler: r={13} p={14} y={15} Temp: {16}",
                AccelXAxis, AccelYAxis, AccelZAxis, GyroXAxis, GyroYAxis, GyroZAxis, MagXAxis, MagYAxis, MagZAxis, QuaternionWAxis, QuaternionXAxis, QuaternionYAxis, QuaternionZAxis, EulerRoll, EulerPitch, EulerYaw, Temperature);
        }

        #endregion
    }
}
