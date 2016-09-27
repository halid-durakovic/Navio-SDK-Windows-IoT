using System;

namespace Emlid.WindowsIot.Hardware.Components.Mpu9250
{
    /// <summary>
    /// Defines the <see cref="Mpu9250ClockSource"/> register of the device.
    /// </summary>
    [Flags]
    public enum Mpu9250ClockSource : byte
    {
        /// <summary>
        /// Internal 20MHz oscillator
        /// </summary>
        Internal = 0x00,

        /// <summary>
        /// Auto selects the best available clock source. If PLL ready otherwise use the internal oscillator.
        /// </summary>
        AutoSelect = 0x01,

        /// <summary>
        /// Disables the clock and keeps timing generator in reset.
        /// </summary>
        Disabled = 0x07,
    }
}
