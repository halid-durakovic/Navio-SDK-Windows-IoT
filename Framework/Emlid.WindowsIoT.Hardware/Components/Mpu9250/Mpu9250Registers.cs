
namespace Emlid.WindowsIot.Hardware.Components.Mpu9250
{
    /// <summary>
    /// Defines the registers of the <see cref="Mpu9250"/>.
    /// </summary>
    /// <remarks>
    /// The reset value is 0x00 for all registers other than <see cref="PowerManagment1"/> and <see cref="WhoAmI"/>.
    /// </remarks>
    public enum Mpu9250Register : byte
    {
        #region SelfTest

        /// <summary>
        /// Register indicates the x-axis gyroscope self test output generate during manufacturing tests (SELF_TEST_X_GYRO). 
        /// </summary>
        /// <remarks>
        /// Value is used to check against subsequent self test.		
        /// </remarks>
        SelfTestXGyro = 0x00,

        /// <summary>
        /// Register indicates the y-axis gyroscope self test output generate during manufacturing tests. 
        /// </summary>
        /// <remarks>
        /// Value is used to check against subsequent self test.		
        /// </remarks>
        SelfTestYGyro = 0x01,

        /// <summary>
        /// Register indicates the z-axis gyroscope self test output generate during manufacturing tests. 
        /// </summary>
        /// <remarks>
        /// Value is used to check against subsequent self test.		
        /// </remarks>
        SelfTestZGyro = 0x02,

        /// <summary>
        /// Register indicates the x-axis accelerometer self test output generate during manufacturing tests. 
        /// </summary>
        /// <remarks>
        /// Value is used to check against subsequent self test.		
        /// </remarks>
        SelfTestXAccel = 0x0D,

        /// <summary>
        /// Register indicates the x-axis accelerometer self test output generate during manufacturing tests. 
        /// </summary>
        /// <remarks>
        /// Value is used to check against subsequent self test.		
        ///</remarks>
        SelfTestYAccel = 0x0E,

        /// <summary>
        /// Register indicates the z-axis accelerometer self test output generate during manufacturing tests. 
        /// </summary>
        /// <remarks>
        /// Value is used to check against subsequent self test.		
        /// </remarks>
        SelfTestZAccel = 0x0F,

        #endregion

        #region Offset

        /// <summary>
        /// High byte of 16-bit word for gyroscope x-axis offset.
        /// </summary>
        OffsetXGHigh = 0x13,
        /// <summary>
        /// Low byte of 16-bit word for gyroscope x-axis offset.
        /// </summary>
        OffsetXGLow = 0x14,

        /// <summary>
        /// High byte of 16-bit word for gyroscope y-axis offset.
        /// </summary>		
        OffsetYGHigh = 0x15,

        /// <summary>
        /// Low byte of 16-bit word for gyroscope y-axis offset.
        /// </summary>
        OffsetYGLow = 0x16,

        /// <summary>
        /// High byte of 16-bit word for gyroscope z-axis offset.
        /// </summary>
        OffsetZGHigh = 0x17,

        /// <summary>
        /// Low byte of 16-bit word for gyroscope z-axis offset.
        /// </summary>
        OffsetZGLow = 0x18,

        /// <summary>
        /// High byte of 16-bit word for accelerometer x-axis offset.
        /// </summary>
        OffsetXAHigh = 0x77,

        /// <summary>
        /// Low byte of 16-bit word for accelerometer  x-axis offset.
        /// </summary>		
        OffsetXALow = 0x78,

        /// <summary>
        /// High byte of 16-bit word for accelerometer y-axis offset.
        /// </summary>		
        OffsetYAHigh = 0x7A,

        /// <summary>
        /// Low byte of 16-bit word for accelerometer y-axis offset.
        /// </summary>
        OffsetYALow = 0x7B,

        /// <summary>
        /// High byte of 16-bit word for accelerometer z-axis offset.
        /// </summary>
        OffsetZAHigh = 0x7D,

        /// <summary>
        /// Low byte of 16-bit word for accelerometer X-axis offset.
        /// </summary>
        OffsetZALow = 0x7E,

        #endregion

        #region Measurements

        /// <summary>
        /// High byte of 16-bit word for accelerometer x-axis data.
        /// </summary>
        AccelDataXHigh = 0x3B,

        /// <summary>
        /// Low byte of 16-bit word for accelerometer x-axis data.
        /// </summary>
        AccelDataXLow = 0x3C,

        /// <summary>
        /// High byte of 16-bit word for accelerometer y-axis data.
        /// </summary>
        AccelDataYHigh = 0x3D,

        /// <summary>
        /// Low byte of 16-bit word for accelerometer y-axis data.
        /// </summary>		
        AccelDataYLow = 0x3E,

        /// <summary>
        /// High byte of 16-bit word for accelerometer z-axis data.
        /// </summary>		
        AccelDataZHigh = 0x3F,

        /// <summary>
        /// Low byte of 16-bit word for accelerometer z-axis data.
        /// </summary>		
        AccelDataZLow = 0x40,

        /// <summary>
        /// High byte of 16-bit word for a temperature sensor data.
        /// </summary>	
        TemperatureDataHigh = 0x41,

        /// <summary>
        /// Low byte of 16-bit word for a temperature sensor data.
        /// </summary>	
        TemperatureDataLow = 0x42,

        /// <summary>
        /// High byte of 16-bit word for gyroscope x-axis data.
        /// </summary>	
        GyroDataXHigh = 0x43,

        /// <summary>
        /// Low byte of 16-bit word for gyroscope x-axis data.
        /// </summary>	
        GyroDataXLow = 0x44,

        /// <summary>
        /// High byte of 16-bit word for gyroscope y-axis data.
        /// </summary>	
        GyroDataYHigh = 0x45,

        /// <summary>
        /// Low byte of 16-bit word for gyroscope y-axis data.
        /// </summary>	
        GyroDataYLow = 0x46,

        /// <summary>
        /// High byte of 16-bit word for gyroscope z-axis data.
        /// </summary>	
        GyroDataZHigh = 0x47,

        /// <summary>
        /// Low byte of 16-bit word for gyroscope z-axis data.
        /// </summary>	
        GyroDataZLow = 0x48,

        #endregion

        #region Configuration

        /// <summary>
        /// Internal sample rate divider (see register CONFIG) to generate the
        /// sample rate that controls sensor data output rate, FIFO sample rate.
        /// </summary>
        SampleRateDiv = 0x19,

        /// <summary>
        /// Register configuration.
        /// </summary>
        /// <remarks>
        /// If set to '1', when the fifo is full, additional writes will not be written to fifo.
        /// When set to '0', when the fifo is full, additional writes will be written to the fifo,
        /// replacing the oldest data.
        /// </remarks>
        Config = 0x1A,

        /// <summary>
        /// Register configuration for the gyroscope. 
        /// </summary>
        GyroConfig = 0x1B,

        /// <summary>
        /// Register configuration for the accelerometer. 
        /// </summary>
        AccelConfig = 0x1C,

        /// <summary>
        /// Register configuration 2 for the accelerometer. 
        /// </summary>
        AccelConfig2 = 0x1D,

        /// <summary>
        /// Register sets the frequency of the low power waking up Output Data Rate(ODR) the devices uses to take a sample of accelerometer data. 
        /// </summary>
        AccelLowPowerODR = 0x1E,

        /// <summary>
        /// Register threshold value for the Wake on Motion Interrupt for accel x/y/z axes. 
        /// </summary>
        WOMThreshold = 0x1F,

        /// <summary>
        /// Enables and disables gryoscope and accelerometer funcations. 
        /// </summary>
        FIFOEnable = 0x23,

        /// <summary>
        /// Enables and disables multi-master capability. 
        /// </summary>
        I2cMasterControl = 0x24,

        #endregion

        #region I2C Control

        /// <summary>
        /// Slave 0 physical address. 
        /// </summary>
        I2cSlave0Address = 0x25,

        /// <summary>
        /// Slave 0 register address from where to begin data transfer. 
        /// </summary>
        I2cSlave0Register = 0x26,

        /// <summary>
        /// Data to be written to Slave 0 control if enabled.
        /// </summary>
        I2cSlave0DO = 0x63,

        /// <summary>
        /// Enables and disables reading data from Slave 0 at the sample rate and storing data
        /// at the first available external sensor data register. 
        /// </summary>
        I2cSlave0Control = 0x27,

        /// <summary>
        /// Slave 1 physical address. 
        /// </summary>
        I2cSlave1Address = 0x28,

        /// <summary>
        /// Slave 1 register address from where to begin data transfer. 
        /// </summary>		
        I2cSlave1Register = 0x29,

        /// <summary>
        /// Data to be written to Slave 1 control if enabled.
        /// </summary>
        I2cSlave1DO = 0x64,

        /// <summary>
        /// Enables and disables reading data from Slave 1 at the sample rate and storing data
        /// at the first available external sensor data register. 
        /// </summary>
        I2cSlave1Control = 0x2A,

        /// <summary>
        /// Slave 2 physical address. 
        /// </summary>		
        I2cSlave2Address = 0x2B,

        /// <summary>
        /// Slave 2 register address from where to begin data transfer. 
        /// </summary>
        I2cSlave2Register = 0x2C,

        /// <summary>
        /// Data to be written to Slave 2 control if enabled.
        /// </summary>
        I2cSlave2DO = 0x65,

        /// <summary>
        /// Enables and disables reading data from Slave 2 at the sample rate and storing data
        /// at the first available external sensor data register. 
        /// </summary>
        I2cSlave2Control = 0x2D,

        /// <summary>
        /// Slave 3 physical address. 
        /// </summary>
        I2cSlave3Address = 0x2E,

        /// <summary>
        /// Slave 3 register address from where to begin data transfer. 
        /// </summary>		
        I2cSlave3Register = 0x2F,

        /// <summary>
        /// Data to be written to Slave 3 control if enabled.
        /// </summary>		
        I2cSlave3DO = 0x66,

        /// <summary>
        /// Enables and disables reading data from Slave 3 at the sample rate and storing data
        /// at the first available external sensor data register. 
        /// </summary>
        I2cSlave3Control = 0x30,

        /// <summary>
        /// Slave 4 physical address. 
        /// </summary>		
        I2cSlave4Address = 0x31,

        /// <summary>
        /// Slave 4 register address from where to begin data transfer. 
        /// </summary>
        I2cSlave4Register = 0x32,

        /// <summary>
        /// Data to be written to Slave 4 control if enabled.
        /// </summary>		
        I2cSlave4DO = 0x33,

        /// <summary>
        /// Enables and disables reading data from Slave 4 at the sample rate and storing data
        /// at the first available external sensor data register. 
        /// </summary>
        I2cSlave4Control = 0x34,

        /// <summary>
        /// Data read from Slave 4 control.
        /// </summary>	
        I2cSlave4DI = 0x35,

        /// <summary>
        /// Master Status.
        /// </summary>	
        I2cMasterStatus = 0x36,

        /// <summary>
        /// Register delays shadowing of external sensor data until all data is received.
        /// </summary>
        I2cMasterDelayControl = 0x67,

        #endregion

        #region Interrupt

        /// <summary>
        /// Enables and disables INT pin bypass. 
        /// </summary>
        INTPinConfig = 0x37,

        /// <summary>
        /// Enables and disables interrupt features. 
        /// </summary>
        INTEnable = 0x38,

        /// <summary>
        /// Interrupt Status. 
        /// </summary>
        INTStatus = 0x3A,

        #endregion

        #region External Sensor Data

        /// <summary>
        /// External sensor data read from external devices via the I2C master interface.
        /// </summary>
        ExternalSensorData00 = 0x49,

        /// <summary>
        /// External sensor data read from external devices via the I2C master interface.
        /// </summary>
        ExternalSensorData01 = 0x4A,

        /// <summary>
        /// External sensor data read from external devices via the I2C master interface.
        /// </summary>
        ExternalSensorData02 = 0x4B,

        /// <summary>
        /// External sensor data read from external devices via the I2C master interface.
        /// </summary>
        ExternalSensorData03 = 0x4C,

        /// <summary>
        /// External sensor data read from external devices via the I2C master interface.
        /// </summary>
        ExternalSensorData04 = 0x4D,

        /// <summary>
        /// External sensor data read from external devices via the I2C master interface.
        /// </summary>
        ExternalSensorData05 = 0x4E,

        /// <summary>
        /// External sensor data read from external devices via the I2C master interface.
        /// </summary>
        ExternalSensorData06 = 0x4F,

        /// <summary>
        /// External sensor data read from external devices via the I2C master interface.
        /// </summary>
        ExternalSensorData07 = 0x50,

        /// <summary>
        /// External sensor data read from external devices via the I2C master interface.
        /// </summary>
        ExternalSensorData08 = 0x51,

        /// <summary>
        /// External sensor data read from external devices via the I2C master interface.
        /// </summary>
        ExternalSensorData09 = 0x52,

        /// <summary>
        /// External sensor data read from external devices via the I2C master interface.
        /// </summary>
        ExternalSensorData10 = 0x53,

        /// <summary>
        /// External sensor data read from external devices via the I2C master interface.
        /// </summary>
        ExternalSensorData11 = 0x54,

        /// <summary>
        /// External sensor data read from external devices via the I2C master interface.
        /// </summary>
        ExternalSensorData12 = 0x55,

        /// <summary>
        /// External sensor data read from external devices via the I2C master interface.
        /// </summary>
        ExternalSensorData13 = 0x56,

        /// <summary>
        /// External sensor data read from external devices via the I2C master interface.
        /// </summary>
        ExternalSensorData14 = 0x57,

        /// <summary>
        /// External sensor data read from external devices via the I2C master interface.
        /// </summary>
        ExternalSensorData15 = 0x58,

        /// <summary>
        /// External sensor data read from external devices via the I2C master interface.
        /// </summary>
        ExternalSensorData16 = 0x59,

        /// <summary>
        /// External sensor data read from external devices via the I2C master interface.
        /// </summary>
        ExternalSensorData17 = 0x5A,

        /// <summary>
        /// External sensor data read from external devices via the I2C master interface.
        /// </summary>
        ExternalSensorData18 = 0x5B,

        /// <summary>
        /// External sensor data read from external devices via the I2C master interface.
        /// </summary>
        ExternalSensorData19 = 0x5C,

        /// <summary>
        /// External sensor data read from external devices via the I2C master interface.
        /// </summary>
        ExternalSensorData20 = 0x5D,

        /// <summary>
        /// External sensor data read from external devices via the I2C master interface.
        /// </summary>
        ExternalSensorData21 = 0x5E,

        /// <summary>
        /// External sensor data read from external devices via the I2C master interface.
        /// </summary>
        ExternalSensorData22 = 0x5F,

        /// <summary>
        /// External sensor data read from external devices via the I2C master interface.
        /// </summary>
        ExternalSensorData23 = 0x60,

        #endregion

        #region General Controls

        /// <summary>
        /// Resets gyroscope, accelerometer and temperature sensor signal paths.
        /// </summary>
        SignalPathReset = 0x68,

        /// <summary>
        /// Enables and disables the Wake-on-Motion detection logic. 
        /// </summary>
        MOTDetectControl = 0x69,

        /// <summary>
        /// Enables and disables user control features. 
        /// </summary>
        UserControl = 0x6A,

        /// <summary>
        /// Power managment control features including reset and sleep. 
        /// </summary>
        PowerManagment1 = 0x6B,

        /// <summary>
        /// Power managment control features including disablement of individual sensor axess. 
        /// </summary>
        PowerManagment2 = 0x6C,

        /// <summary>
        /// High byte of 16-bit word count for the number of written bytes in the FIFO.
        /// </summary>	
        FIFOCountHigh = 0x72,

        /// <summary>
        /// Low byte of 16-bit word count for the number of written bytes in the FIFO.
        /// </summary>	
        FIFOCountLow = 0x73,

        /// <summary>
        /// Register is used to read and write data from the FIFO buffer.
        /// </summary>	
        FIFOReadWrite = 0x74,

        /// <summary>
        /// Register is used to verify the identity of the device.
        /// </summary>
        /// <remarks>
        ///The contents is an 8-bit device ID. The default value of the register is 0x71.		
        ///</remarks>
        WhoAmI = 0x75,

        #endregion

    }
}
