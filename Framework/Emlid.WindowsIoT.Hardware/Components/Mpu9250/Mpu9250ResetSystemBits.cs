using System;

namespace Emlid.WindowsIot.Hardware.Components.Mpu9250
{
    /// <summary>
    /// Bitmask for the <see cref="Mpu9250Register.PowerManagment1"/> register.
    /// </summary>
    [Flags]
    public enum Mpu9250ResetSystemBits : byte
    {
        /// <summary>
        /// Reset the internal registers and restores the default settings. The bit will auto clear after reset.
        /// </summary>
        Enable = 0x80
    }
}
