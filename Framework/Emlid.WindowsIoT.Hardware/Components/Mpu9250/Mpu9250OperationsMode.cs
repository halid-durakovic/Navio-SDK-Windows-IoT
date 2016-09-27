using System;

namespace Emlid.WindowsIot.Hardware.Components.Mpu9250
{
    /// <summary>
    /// Defines the <see cref="Mpu9250Device.OperationMode"/> register of the device.
    /// </summary>
    [Flags]
    public enum Mpu9250OperationsMode : byte
    {

        #region Configuration

        /// <summary>
        /// Mode is used to configure device.  All output data is reset to zero and sensor
        /// fusion is halted.  This is the only mode in which all the writable register map entrids can be change.
        /// </summary>
        Config = 0x00,

        #endregion

        #region Non-fusion Modes

        /// <summary>
        /// Device behaves like a stand-alone accelerometer sensor, with mangetometer
        /// and gyroscope sensors being suspended.
        /// </summary>
        AccelOnly = 0x01,

        /// <summary>
        /// Device behaves like a stand-alone mangetometer sensor, with accelerometer
        /// and gyroscope sensors being suspended.
        /// </summary>
        MagOnly = 0x02,

        /// <summary>
        /// Device behaves like a stand-alone gyroscope sensor, with accelerometer
        /// and mangetometer sensors being suspended.
        /// </summary>
        GyroOnly = 0x03,

        /// <summary>
        /// Accelerometer and mangetometer sensors are active and the gyroscope sensor is suspended. 
        /// </summary>
        AccelMag = 0x04,

        /// <summary>
        /// Accelerometer and gyroscope sensors are active and the mangetometer sensor is suspended. 
        /// </summary>
        AccelGyro = 0x05,

        /// <summary>
        /// Mangetometer and gyroscope sensors are active and the accelerometer sensor is suspended. 
        /// </summary>
        MagGyro = 0x06,

        /// <summary>
        /// The accelerometer, mangetometer and gyroscope sensors are all active. 
        /// </summary>
        AccelGyroMag = 0x07,

        #endregion

        #region Fusion Modes

        /// <summary>
        /// Fusion mode with 9 degrees of freedom where the fused absolute orientation data is calculated from the accelerometer, gyroscope
        /// and the magnetometer. The advantages of combining all three sensors are a fast calculation, resulting in high output data rate,
        /// and high robustness from magnetic field distortion. 
        /// </summary>
        Fusion = 0x08,


        /// <summary>
        /// Fusion mode with 6 degrees of freedom where the fused absolute orientation data is calculated from the accelerometer and gyroscope.  
        /// </summary>
        Fusion2 = 0x09

        #endregion
    }
}
