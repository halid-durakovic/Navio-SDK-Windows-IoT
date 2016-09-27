
namespace Emlid.WindowsIot.Hardware.Components.Mpu9250
{
    /// <summary>
    /// Defines the registers of the <see cref="Mpu9250"/>.
    /// </summary>
    public enum Mpu9250MagRegister : byte
    {

        #region Magnetometer (AK8963)

        /// <summary>
        /// AK8963 I2C slave address.
        /// </summary>
        Ak8963I2cAddress = 0x0c,

        // Read-only Registers

        /// <summary>
        /// Register indicates the device id (WIA) of the magnetometer.
        /// </summary>
        MagWhoAmI = 0x00,

        /// <summary>
        /// Register indicates the device information of the magnetometer.
        /// </summary>
        MagInfo = 0x01,

        /// <summary>
        /// Register indicates the data ready status the magnetometer.
        /// </summary>
        MagStatus1 = 0x02,

        /// <summary>
        /// Low byte of 16-bit word for magnetometer x-axis data.
        /// </summary>
        MagDataXLow = 0x03,

        /// <summary>
        /// High byte of 16-bit word for magnetometer x-axis data.
        /// </summary>
        MagDataXHigh = 0x04,

        /// <summary>
        /// Low byte of 16-bit word for magnetometer y-axis data.
        /// </summary>
        MagDataYLow = 0x05,

        /// <summary>
        /// High byte of 16-bit word for magnetometer y-axis data.
        /// </summary>
        MagDataYHigh = 0x06,

        /// <summary>
        /// Low byte of 16-bit word for magnetometer z-axis data.
        /// </summary>
        MagDataZLow = 0x07,

        /// <summary>
        /// High byte of 16-bit word for magnetometer z-axis data.
        /// </summary>
        MagDataZHigh = 0x08,

        /// <summary>
        /// Register indicates the magnetic sensor overflow status the magnetometer.
        /// </summary>
        MagStatus2 = 0x09,

        // Write/Read Registers

        /// <summary>
        /// Mode control for the magnetometer.
        /// </summary>
        /// <remarks>
        /// When register is accessed to be written, registers from 0x02 to 0x09 are initialized.
        /// </remarks>
        MagControl1 = 0x0A,

        /// <summary>
        /// Register soft resets the magnetometer.
        /// </summary>
        /// <remarks>
        /// When '1' is set, all registers are initialized.
        /// </remarks>
        MagControl2 = 0x0B,

        /// <summary>
        /// Self test control the magnetometer.
        /// </summary>
        /// <remarks>
        /// Do not write '1' to any bit other than SELF bit in register.
        /// </remarks>
        MagSelfTest = 0x0C,

        /// <summary>
        /// Register is a test register used for magnetometer shipment test.
        /// </summary>
        /// <remarks>
        ///  Do not use this register.
        /// </remarks>
        MagTest1 = 0x0D,

        /// <summary>
        /// Register is a test register used for magnetometer shipment test.
        /// </summary>
        /// <remarks>
        ///  Do not use this register.
        /// </remarks>
        MagTest2 = 0x0E,

        /// <summary>
        /// Register disables the I2C bus interface of the magnetometer.
        /// </summary>
        /// <remarks>
        /// To disable bus inteface write '00011011" to register. To enable
        /// reset magnetomter or input start condition 8 times continuously.		
        /// </remarks>
        MagI2CDisable = 0x0F,

        // Read-only Registers (ROM)

        /// <summary>
        /// Sensitivity adjustment data for each x-axis is stored to fuse ROM on shipment.
        /// </summary>
        /// <remarks>
        /// Values can be read only in fuse access mode <see cref="MagControl1"/>.		
        /// </remarks>
        MagXSensitivity = 0x10,

        /// <summary>
        /// Sensitivity adjustment data for each y-axis is stored to fuse ROM on shipment.
        /// </summary>
        /// <remarks>
        /// Values can be read only in fuse access mode <see cref="MagControl1"/>.		
        /// </remarks>
        MagYSensitivity = 0x11,

        /// <summary>
        /// Sensitivity adjustment data for each z-axis is stored to fuse ROM on shipment.
        /// </summary>
        /// <remarks>
        /// Values can be read only in fuse access mode <see cref="MagControl1"/>.		
        /// </remarks>
        MagZSensitivity = 0x12

        #endregion

    }
}
