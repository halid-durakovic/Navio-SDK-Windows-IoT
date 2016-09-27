using System;

namespace Emlid.WindowsIot.Hardware.Components.Mpu9250
{
    /// <summary>
    /// Bitmask for the <see cref="Mpu9250Register.GyroConfig"/> register.
    /// </summary>
    [Flags]
    public enum Mpu9250ConfigGyroBits : byte
    {
        /// <summary>
        /// Used to bypass DLPF.
        /// </summary>
        FChoice = 0x03,

        /// <summary>
        /// Selects gyroscope full scale.
        /// </summary>
        GyroScaleSelect = 0x18,

        /// <summary>
        /// Enables or disables z-axis gyroscope selftest.
        /// </summary>
        GyroZSelftest = 0x20,

        /// <summary>
        /// Enables or disables y-axis gyroscope selftest.
        /// </summary>
        GyroYZSelftest = 0x40,

        /// <summary>
        /// Enables or disables x-axis gyroscope selftest.
        /// </summary>
        GyroXSelftest = 0x80,
    }
}
