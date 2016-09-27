using System;

namespace Emlid.WindowsIot.Hardware.Components.Mpu9250
{
    /// <summary>
    /// Defines the <see cref="Mpu9250"/> register of the device.
    /// </summary>
    [Flags]
    public enum Mpu9250MagMode : byte
    {
        /// <summary>
        /// Power-down mode.
        /// </summary>
        PowerDown = 0x00,

        /// <summary>
        /// Single measurement mode.
        /// </summary>
        Single = 0x01,

        /// <summary>
        /// Continuous measurement mode 1.
        /// </summary>
        ContinuousMode1 = 0x02,

        /// <summary>
        /// Continuous measurement mode 2.
        /// </summary>
        ContinuousMode2 = 0x06,

        /// <summary>
        /// External trigger measurement mode.
        /// </summary>
        ExternalTrigger = 0x04,

        /// <summary>
        /// Self-test mode.
        /// </summary>
        SelfTest = 0x08,

        /// <summary>
        /// Fuse ROM access mode.
        /// </summary>
        FuseROM = 0x0F,

    }
}
