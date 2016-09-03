using System;

namespace Emlid.WindowsIot.Hardware.Components.Mpu9250
{
    /// <summary>
    /// Bitmask for the <see cref="Mpu9250Register.PowerManagment1"/> register.
    /// </summary>
    [Flags]
    public enum Mpu9250ClockSourceBits : byte
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
