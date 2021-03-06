﻿using System;

namespace Emlid.WindowsIot.Hardware.Components.Mpu9250
{
    /// <summary>
    /// Bitmask for the <see cref="Mpu9250Register.PowerManagment2"/> register.
    /// </summary>
    [Flags]
    public enum Mpu9250PowerManagment2Bits : byte
    {
        /// <summary>
        /// Enables or disables the accelerometer and gyroscope.
        /// </summary>
        AccelGyro = 0x3F,

        /// <summary>
        /// Enables or disables gyroscope.
        /// </summary>
        Gyro = 0x07,

        /// <summary>
        /// Enables or disables z-axis gyroscope.
        /// </summary>
        GyroZ = 0x01,

        /// <summary>
        /// Enables or disables y-axis gyroscope.
        /// </summary>
        GyroY = 0x02,

        /// <summary>
        /// Enables or disables x-axis gyroscope.
        /// </summary>
        GyroX = 0x04,

        /// <summary>
        /// Enables or disables the accelerometer.
        /// </summary>
        Accel = 0x04,

        /// <summary>
        /// Enables or disables z-axis accelerometer.
        /// </summary>
        AccelZ = 0x08,

        /// <summary>
        /// Enables or disables y-axis accelerometer.
        /// </summary>
        AccelY = 0x10,

        /// <summary>
        /// Enables or disables x-axis accelerometer.
        /// </summary>
        AccelX = 0x20,
    }
}
