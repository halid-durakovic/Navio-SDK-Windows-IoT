using System;

namespace Emlid.WindowsIot.Hardware.Components.Mpu9250
{
    /// <summary>
    /// Defines the <see cref="Mpu9250ConfigGyroBits.GyroScaleSelect"/> register of the device.
    /// </summary>
    [Flags]
    public enum Mpu9250GyroScale: byte
    {
        /// <summary>
        /// Selects gyroscope scale of ±250°/sec (dps).
        /// </summary>
        Scale250Dbps = 0x00,

        /// <summary>
        /// Selects gyroscope scale of ±500°/sec (dps).
        /// </summary>
        Scale500Dbps = 0x08,

        /// <summary>
        /// Selects gyroscope scale of ±1000°/sec (dps).
        /// </summary>
        Scale1000Dbps = 0x10,

        /// <summary>
        /// Selects gyroscope scale of ±2000°/sec (dps).
        /// </summary>
        Scale2000Dbps = 0x18
    }
}
