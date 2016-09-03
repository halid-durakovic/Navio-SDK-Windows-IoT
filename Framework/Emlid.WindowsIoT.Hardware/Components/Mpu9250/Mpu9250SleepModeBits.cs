using System;

namespace Emlid.WindowsIot.Hardware.Components.Mpu9250
{
    /// <summary>
    /// Bitmask for the <see cref="Mpu9250Register.PowerManagment1"/> register.
    /// </summary>
    [Flags]
    public enum Mpu9250SleepModeBits : byte
    {
        /// <summary>
        /// The device is woke up from sleep mode. 
        /// </summary>
        Wake = 0x00,

        /// <summary>
        /// The device is set to sleep mode.
        /// </summary>
        Sleep = 0x40
    }
}
