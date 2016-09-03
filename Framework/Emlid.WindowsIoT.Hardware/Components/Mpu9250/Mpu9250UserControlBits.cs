using System;

namespace Emlid.WindowsIot.Hardware.Components.Mpu9250
{
    /// <summary>
    /// Bitmask for the <see cref="Mpu9250Register.UserControl"/> register.
    /// </summary>
    [Flags]
    public enum Mpu9250UserControlBits : byte
    {

        /// <summary>
        /// Reset all gyro digital signal path, accel digital signal path, and temp digital signal path. This bit also clears all the sensor registers.
        /// </summary>
        ResetSignal = 0x01,

        /// <summary>
        /// Reset I2C Master module. Reset is asynchronous. This bit auto clears after one clock cycle.
        /// This bit should only be set when the I2C master has hung. If this bit is set during an active I2C master transaction, the I2C
        /// slave will hang, which will require the host to reset the slave.
        /// </summary>
        ResetI2cMaster = 0x02,

        /// <summary>
        /// Reset FIFO module. Reset is asynchronous. This bit auto clears after one clock cycle.
        /// </summary>
        ResetFIFO = 0x04,

        /// <summary>
        /// Disable I2C Slave module and put the serial interface in SPI mode only.
        /// </summary>
        DisableI2cSlave = 0x10,

        /// <summary>
        /// Enable(1) / Disable(0) the I2C Master I/F module; pins ES_DA and ES_SCL are isolated from pins SDA/SDI and SCL/ SCLK.
        /// DMP will run when enabled, even if all internal sensors are disabled, except when the sample rate is set to 8Khz.
        /// </summary>
        EnableI2cMaster = 0x20,

        /// <summary>
        /// Enable(1) / Disable(0) FIFO access from serial interface. To disable FIFO writes by dma, use FIFO_EN register. To disable 
        /// possible FIFO writes from DMP, disable the DMP.
        /// </summary>
        EnableFIFO = 0x40,
    }
}
