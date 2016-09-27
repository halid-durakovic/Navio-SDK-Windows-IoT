using System;

namespace Emlid.WindowsIot.Hardware.Components.Mpu9250
{
    /// <summary>
    /// Bitmask for the <see cref="Mpu9250Register.I2cMasterControl"/> register.
    /// </summary>
    [Flags]
    public enum Mpu9250I2cMasterControlBits : byte
    {
        /// <summary>
        /// A 4 bit unsigned value which configures a divider on the MPU-9250 internal 8MHz clock.
        /// </summary>
        I2cMasterClock = 0xF,

        /// <summary>
        /// Controls the I2C Master’s transition from one slave read to the next slave read. If 0, there is a restart between reads. If 1, there is a stop between reads.
        /// </summary>
        I2cMasterTransition = 0x10,

        /// <summary>
        /// Enable(1) / Disable(0) the write EXT_SENS_DATA registers associated to SLV_3 (as determined by 2C_SLV0_CTRL and I2C_SLV1_CTRL and I2C_SLV2_CTRL) to the FIFO atthe sample rate;
        /// </summary>
        EnableSlvFIFO = 0x20,

        /// <summary>
        /// Delays the data ready interrupt until external sensor data is loaded. If I2C_MST_IF is disabled, the interrupt will still occur.
        /// </summary>
        WaitForES = 0x40,

        /// <summary>
        ///  Enable(1) / Disable(0) multi-master capability. When disabled, clocking to the I2C_MST_IF can be disabled when not in use and the logic to detect lost arbitration is disabled.
        /// </summary>
        EnableMultimaster = 0x80,
    }
}
