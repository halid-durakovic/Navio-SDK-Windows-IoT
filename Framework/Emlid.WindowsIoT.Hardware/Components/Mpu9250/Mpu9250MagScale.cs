using System;

namespace Emlid.WindowsIot.Hardware.Components.Mpu9250
{
    /// <summary>
    /// Defines the <see cref="Mpu9250"/> register of the device.
    /// </summary>
    [Flags]
    public enum Mpu9250MagScale : byte
    {
        /// <summary>
        /// Enables magnetometer continuous mesurement in 14bit with a scale of 0.6 mG per LSB.
        /// </summary>
        Scale14Bits = 0x00,

        /// <summary>
        /// Selects magnetometer continuous mesurement in 16bit with a scale of 0.15 mG per LSB.
        /// </summary>
        Scale16Bits = 0x10,
    }
}
