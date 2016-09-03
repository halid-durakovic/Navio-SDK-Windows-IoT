using System;

namespace Emlid.WindowsIot.Hardware.Components.Mpu9250
{
    /// <summary>
    /// Bitmask for the <see cref="Mpu9250Register.ConfigAccel"/> register.
    /// </summary>
    [Flags]
    public enum Mpu9250ConfigAccelBits : byte
    {
        /// <summary>
        ///Selects accelerometer full scale.
        /// </summary>
        AccelScaleSelect = 0x18,

        /// <summary>
        /// Enables or disables x-axis accelerometer selftest.
        /// </summary>
        AccelXSelftest = 0x20,

        /// <summary>
        /// Enables or disables y-axis accelerometer selftest.
        /// </summary>
        AccelYZSelftest = 0x40,

        /// <summary>
        /// Enables or disables z-axis accelerometer selftest.
        /// </summary>
        AccelZSelftest = 0x80
    }
}
