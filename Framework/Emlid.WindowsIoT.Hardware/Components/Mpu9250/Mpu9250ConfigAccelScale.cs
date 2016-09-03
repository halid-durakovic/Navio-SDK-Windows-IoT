using System;

namespace Emlid.WindowsIot.Hardware.Components.Mpu9250
{
    /// <summary>
    /// Defines the <see cref="Mpu9250ConfigAccelBits.AccelScaleSelect"/> register of the device.
    /// </summary>
    [Flags]
    public enum Mpu9250ConfigAccelScale: byte
    {
        /// <summary>
        /// Selects accelerometer scale of ±2g.
        /// </summary>
        Scale2G = 0x00,

        /// <summary>
        /// Selects accelerometer scale of ±4g.
        /// </summary>
        Scale4G = 0x08,

        /// <summary>
        /// Selects accelerometer scale of ±8g.
        /// </summary>
        Scale8G = 0x10,

        /// <summary>
        /// Selects accelerometer scale of ±16g.
        /// </summary>
        Scale16G = 0x18
    }
}
