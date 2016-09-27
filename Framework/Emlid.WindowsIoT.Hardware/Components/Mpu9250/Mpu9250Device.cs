using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Windows.Devices.Spi;
using Windows.Foundation;

using System.Diagnostics;
using Windows.Devices.Gpio;

using Emlid.WindowsIot.Common;
using Emlid.WindowsIot.Hardware.Filters;

namespace Emlid.WindowsIot.Hardware.Components.Mpu9250
{

    /// <summary>
    /// MPU-9250 9-axis motion tracking sensor.
    /// </summary>
    public class Mpu9250Device : DisposableObject
    {
        #region Constants

        /// <summary>
        /// Time to wait for the <see cref="Reset"/> command to complete in millisecond.
        /// </summary>
        public const int ResetDelay = 650;

        /// <summary>
        /// Time to wait for the device to startup in millisecond.
        /// </summary>
        public const int StartupDelay = 100;

        /// <summary>
        /// Time to wait for the device to switch modes in millisecond.
        /// </summary>
        public const int ModeSwitchDelay = 600;

        /// <summary>
        /// Time to wait after a write for the device to update registers in millisecond.
        /// </summary>
        public const int WriteDelay = 50;

        /// <summary>
        /// The standard acceleration due to gravity in m/s2.
        /// </summary>
        public const float StandardGravity = 9.80665f;

        /// <summary>
        /// Room temperature offset in LSB.
        /// </summary>
        public const float RoomTemperatureOffset = 0;

        /// <summary>
        /// Temperature sensitivity in LSB/°C.
        /// </summary>
        public const float TempSensitivity = 333.87f;

        /// <summary>
        /// Madgwich AHRS algorithm sample period.
        /// </summary>
        public const float MadgwickSamplePeriod = 1f / 256f;

        /// <summary>
        /// Madgwich AHRS algorithm gain beta.
        /// </summary>
        public const float MadgwickBeta = 0.1f;


        #endregion

        #region Lifetime

        /// <summary>
        /// Creates an instance using the specified device and sampling rate.
        /// </summary>
        /// <param name="device">SPI device.</param>
        [CLSCompliant(false)]
        public Mpu9250Device(SpiDevice device) 
        {
            // Validate
            if (device == null) throw new ArgumentNullException(nameof(device));

            // Initialize hardware
            Hardware = device;

            // Reset the device to default setting
            this.Reset();

            // Set clock source
            Hardware.WriteJoinByte((byte)Mpu9250Register.PowerManagment1, 0x01);

            // Set I2C master mode
            Hardware.WriteJoinByte((byte)Mpu9250Register.UserControl, 0x20);

            // Set I2C configureation multi-master IIC 400KHz.
            Hardware.WriteJoinByte((byte)Mpu9250Register.I2cMasterControl, 0x0D);

            // Set temprature bandwidth to 188Hz and gyroscope bandwidth to 184Hz.
            Hardware.WriteJoinByte((byte)Mpu9250Register.Config, 0x00);

            // Verify the device is connected, accessable and ready for use.
            if (ChipId == 0x71 & Ak8963ChipId == 0x48)
                IsConnected = true;

            // Initialize sensor reading
            SensorReading = new Mpu9250SensorReading();

            // Initialize sensor axis offset data
            AxisOffset = new Mpu9250OffsetReading() { MagXAxisSensitivity = 1, MagYAxisSensitivity = 1, MagZAxisSensitivity = 1 };
            
        }

        /// <summary>
        /// <see cref="DisposableObject.Dispose(bool)"/>.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            // Only managed resources to dispose
            if (!disposing)
                return;

            // Close device
            Hardware?.Dispose();
        }

        #endregion

        #region Private Fields

        /// <summary>
        ///  Accelerometer scale resolution multiplier. 
        /// </summary>
        private double _accelMuliplier = 2.0 / 32768.0; // Default 2G scale

        /// <summary>
        ///  Gyroscope scale resolution multiplier. 
        /// </summary>
        private double _gyroMuliplier = 250.0 / 32768.0;  // Default 250Dbps scale

        /// <summary>
        ///  Magnetometer scale resolution multiplier. 
        /// </summary>
        private double _magMuliplier = 10.0 * 4912.0 / 8190.0; // Default 14bit scale 

        /// <summary>
        ///  Accelerometer scale resolution multiplier. 
        /// </summary>
        private Mpu9250AccelScale _accelScale = Mpu9250AccelScale.Scale2G;

        /// <summary>
        ///  Gyroscope scale resolution multiplier. 
        /// </summary>
        private Mpu9250GyroScale _gyroScale = Mpu9250GyroScale.Scale250Dbps;

        /// <summary>
        ///  Magnetometer scale resolution multiplier. 
        /// </summary>
        private Mpu9250MagScale _magScale = Mpu9250MagScale.Scale14Bits;

        /// <summary>
        ///  
        /// </summary>
        private Mpu9250OperationsMode _operationsMode = Mpu9250OperationsMode.Fusion;

        /// <summary>
        ///  
        /// </summary>
        private Mpu9250MagMode _magMode = Mpu9250MagMode.PowerDown;


        /// <summary>
        ///  
        /// </summary>
        private Mpu9250OffsetReading _axisOffset = new Mpu9250OffsetReading();

        #endregion

        #region Public Properties

        /// <summary>
        /// Indicates if the motion sensor is connected, accessable and ready for use.
        /// </summary>
        public bool IsConnected { get; protected set; }

        /// <summary>
        /// MPU9250 chip id data.
        /// </summary>
        public byte ChipId
        {
            get { return Hardware.WriteReadByte((byte)Mpu9250Register.WhoAmI); }
        }

        /// <summary>
        /// AK8963 chip id data.
        /// </summary>
        public byte Ak8963ChipId
        {
            get { return ReadAK8963((byte)Mpu9250MagRegister.MagWhoAmI); }
        }

        /// <summary>
        /// Gets or sets the operations mode of the device.
        /// </summary>
        public Mpu9250OperationsMode OperationMode
        {
            get { return _operationsMode; }
            set
            {
                switch (value)
                {
                    case Mpu9250OperationsMode.Config:

                        // Disable accelerometer and gyroscope
                        Hardware.WriteJoinByte((byte)Mpu9250Register.PowerManagment2, 0x3F);

                        // Set magnetometer to fuse ROM access mode
                        ReadWriteAK8963(Mpu9250MagRegister.MagControl1, (byte)Mpu9250MagMode.FuseROM, true);
                        break;

                    case Mpu9250OperationsMode.AccelOnly:

                        // Set accelerometer data reates, enable accel LPF, and bandwidth 184Hz
                        Hardware.WriteJoinByte((byte)Mpu9250Register.AccelConfig, 0x08);

                        // Enable accelerometer and disable gyroscope
                        Hardware.WriteJoinByte((byte)Mpu9250Register.PowerManagment2, 0x07);

                        // Disable magnetometer 
                        ReadWriteAK8963(Mpu9250MagRegister.MagControl1, (byte)Mpu9250MagMode.ContinuousMode1, false);
                        break;

                    case Mpu9250OperationsMode.MagOnly:

                        // Disable accelerometer and gyroscope
                        Hardware.WriteJoinByte((byte)Mpu9250Register.PowerManagment2, 0x3F);

                        // Enable magnetometer continious mode 1
                        ReadWriteAK8963(Mpu9250MagRegister.MagControl1, (byte)Mpu9250MagMode.ContinuousMode1, true);
                        break;

                    case Mpu9250OperationsMode.GyroOnly:

                        // Enable gyroscope and disable accelerometer
                        Hardware.WriteJoinByte((byte)Mpu9250Register.PowerManagment2, 0x38);

                        // Disable magnetometer
                        ReadWriteAK8963(Mpu9250MagRegister.MagControl1, (byte)Mpu9250MagMode.ContinuousMode1, false);
                        break;

                    case Mpu9250OperationsMode.AccelMag:

                        // Set accelerometer data reates, enable accel LPF, and bandwidth 184Hz
                        Hardware.WriteJoinByte((byte)Mpu9250Register.AccelConfig, 0x08);

                        // Enable accelerometer and disable gyroscope
                        Hardware.WriteJoinByte((byte)Mpu9250Register.PowerManagment2, 0x07);

                        // Enable magnetometer continious mode 1
                        ReadWriteAK8963(Mpu9250MagRegister.MagControl1, (byte)Mpu9250MagMode.ContinuousMode1, true);
                        break;

                    case Mpu9250OperationsMode.AccelGyro:

                        // Set accelerometer data reates, enable accel LPF, and bandwidth 184Hz
                        Hardware.WriteJoinByte((byte)Mpu9250Register.AccelConfig, 0x08);

                        // Enable accelerometer and gyroscope
                        Hardware.WriteJoinByte((byte)Mpu9250Register.PowerManagment2, 0x00);

                        // Disable magnetometer 
                        ReadWriteAK8963(Mpu9250MagRegister.MagControl1, (byte)Mpu9250MagMode.ContinuousMode1, false);
                        break;

                    case Mpu9250OperationsMode.MagGyro:

                        // Enable gyroscope and disable accelerometer
                        Hardware.WriteJoinByte((byte)Mpu9250Register.PowerManagment2, 0x38);

                        // Enable magnetometer continious mode 1
                        ReadWriteAK8963(Mpu9250MagRegister.MagControl1, (byte)Mpu9250MagMode.ContinuousMode1, true);
                        break;

                    case Mpu9250OperationsMode.AccelGyroMag:

                        // Set accelerometer data reates, enable accel LPF, and bandwidth 184Hz
                        Hardware.WriteJoinByte((byte)Mpu9250Register.AccelConfig, 0x08);

                        // Enable accelerometer and gyroscope
                        Hardware.WriteJoinByte((byte)Mpu9250Register.PowerManagment2, 0x00);

                        // Enable magnetometer continious mode 1
                        ReadWriteAK8963(Mpu9250MagRegister.MagControl1, (byte)Mpu9250MagMode.ContinuousMode1, true);
                        break;

                    case Mpu9250OperationsMode.Fusion:

                        // Set accelerometer data reates, enable accel LPF, and bandwidth 184Hz
                        Hardware.WriteJoinByte((byte)Mpu9250Register.AccelConfig, 0x08);

                        // Enable accelerometer and gyroscope
                        //Hardware.WriteJoinByte((byte)Mpu9250Register.PowerManagment2, 0x00);
                        MagMode = Mpu9250MagMode.ContinuousMode1;
                        // Enable magnetometer continious mode 1
                        //WriteMagRegister(Mpu9250MagRegister.MagControl1, 0x12); 
                        //ReadWriteAK8963(Mpu9250MagRegister.MagControl1, (byte)Mpu9250MagMode.ContinuousMode1, true);

                        break;
                }

                Task.Delay(ModeSwitchDelay);

                _operationsMode = value;
            }
        }

        /// <summary>
        /// Sets the remapped axis of the device to standard predefined placement locations.
        /// </summary>
        public Mpu9250Placement Placement
        {
            set
            {
                //TODO:
                switch (value)
                {
                    case Mpu9250Placement.P0:

                        break;
                    case Mpu9250Placement.P1:

                        break;
                    case Mpu9250Placement.P2:

                        break;
                    case Mpu9250Placement.P3:

                        break;
                    case Mpu9250Placement.P4:

                        break;
                    case Mpu9250Placement.P5:

                        break;
                    case Mpu9250Placement.P6:

                        break;
                    case Mpu9250Placement.P7:

                        break;
                }
            }
        }

        /// <summary>
        /// Gets or sets the scale range for the accelerometer.
        /// </summary>
        public Mpu9250AccelScale AccelScale
        {
            get { return _accelScale; }

            set
            {
                // Write to register
                Hardware.WriteReadWriteBit((byte)Mpu9250Register.AccelConfig, (byte)value, true);

                switch (value)
                {
                    case Mpu9250AccelScale.Scale2G:
                        _accelMuliplier =  2.0 / 32768.0;
                        break;
                    case Mpu9250AccelScale.Scale4G:
                        _accelMuliplier =  4.0 / 32768.0;
                        break;
                    case Mpu9250AccelScale.Scale8G:
                        _accelMuliplier =  8.0 / 32768.0;
                        break;
                    case Mpu9250AccelScale.Scale16G:
                        _accelMuliplier =  16.0 / 32768.0;
                        break;
                }
                _accelScale = value;
            }
        }

        /// <summary>
        /// Gets or sets the scale range for the gyroscope.
        /// </summary>
        public Mpu9250GyroScale GyroScale
        {
            get { return _gyroScale; }
            set
            {
                // Write to register
                Hardware.WriteReadWriteBit((byte)Mpu9250Register.GyroConfig, (byte)value, true);

                switch (value)
                {
                    case Mpu9250GyroScale.Scale250Dbps:
                        _gyroMuliplier = 250.0 / 32768.0;
                        break;
                    case Mpu9250GyroScale.Scale500Dbps:
                        _gyroMuliplier = 500.0 / 32768.0;
                        break;
                    case Mpu9250GyroScale.Scale1000Dbps:
                        _gyroMuliplier = 1000.0 / 32758.0;
                        break;
                    case Mpu9250GyroScale.Scale2000Dbps:
                        _gyroMuliplier = 2000.0 / 32768.0;
                        break;
                }
                _gyroScale = value;
            }
        }

        /// <summary>
        /// Gets or sets the output scale range for the magnetometer.
        /// </summary>
        public Mpu9250MagScale MagScale
        {
            get
            { return _magScale; }

            set
            {
                switch (value)
                {
                    case Mpu9250MagScale.Scale14Bits:
                        ReadWriteAK8963(Mpu9250MagRegister.MagControl1, (byte)Mpu9250MagScale.Scale14Bits, true);
                        _magMuliplier = 10.0 * 4912.0 / 8190.0; // Proper scale to return milliGauss
                        break;

                    case Mpu9250MagScale.Scale16Bits:
                        ReadWriteAK8963(Mpu9250MagRegister.MagControl1, (byte)Mpu9250MagScale.Scale16Bits, true);
                        _magMuliplier = 10.0 * 4912.0 / 32760.0; // Proper scale to return milliGauss
                        break;
                }
                _magScale = value;
            }
        }

        /// <summary>
        /// Gets or sets the operating mode for the magnetometer.
        /// </summary>
        public Mpu9250MagMode MagMode
        {
            get
            { return _magMode; }

            set
            {
                ReadWriteAK8963(Mpu9250MagRegister.MagControl1, (byte)value, true);
                _magMode = value;
            }
        }

        /// <summary>
        /// Gets or sets the accelerometer, gyroscope and magnetometer axis offset.
        /// </summary>>
        public Mpu9250OffsetReading AxisOffset
        {
            get { return _axisOffset; }
            set { _axisOffset = value; }
        }

        /// <summary>
        /// Result of the last <see cref="Update"/>.
        /// </summary>>
        public Mpu9250SensorReading SensorReading { get; protected set; }

        #region Register Access Properties

        /// <summary>
        /// Gets or sets the sample rate divider configuration register 25.
        /// </summary>
        public byte Register25
        {
            get { return Hardware.WriteReadByte((byte)Mpu9250Register.SampleRateDiv); }
            set { Hardware.WriteJoinByte((byte)Mpu9250Register.SampleRateDiv, value); }
        }

        /// <summary>
        /// Gets or sets the configuration register 26.
        /// </summary>
        public byte Register26
        {
            get { return Hardware.WriteReadByte((byte)Mpu9250Register.Config); }
            set { Hardware.WriteJoinByte((byte)Mpu9250Register.Config, value); }
        }

        /// <summary>
        /// Gets or sets the gyroscope configuration register 27.
        /// </summary>
        public byte Register27
        {
            get { return Hardware.WriteReadByte((byte)Mpu9250Register.GyroConfig); }
            set { Hardware.WriteJoinByte((byte)Mpu9250Register.GyroConfig, value); }
        }

        /// <summary>
        /// Gets or sets the accelerometer configuration register 28.
        /// </summary>
        public byte Register28
        {
            get { return Hardware.WriteReadByte((byte)Mpu9250Register.AccelConfig); }
            set { Hardware.WriteJoinByte((byte)Mpu9250Register.AccelConfig, value); }
        }

        /// <summary>
        /// Gets or sets the accelerometer configuration register 29.
        /// </summary>
        public byte Register29
        {
            get { return Hardware.WriteReadByte((byte)Mpu9250Register.AccelConfig2); }
            set { Hardware.WriteJoinByte((byte)Mpu9250Register.AccelConfig2, value); }
        }

        /// <summary>
        /// Gets or sets the low power accelerometer ODR control configuration register 30.
        /// </summary>
        public byte Register30
        {
            get { return Hardware.WriteReadByte((byte)Mpu9250Register.AccelLowPowerODR); }
            set { Hardware.WriteJoinByte((byte)Mpu9250Register.AccelLowPowerODR, value); }
        }

        /// <summary>
        /// Gets or sets the wake-on motion threshold configuration register 31.
        /// </summary>
        public byte Register31
        {
            get { return Hardware.WriteReadByte((byte)Mpu9250Register.WOMThreshold); }
            set { Hardware.WriteJoinByte((byte)Mpu9250Register.WOMThreshold, value); }
        }

        /// <summary>
        /// Gets or sets the FIFO enable configuration register 35.
        /// </summary>
        public byte Register35
        {
            get { return Hardware.WriteReadByte((byte)Mpu9250Register.FIFOEnable); }
            set { Hardware.WriteJoinByte((byte)Mpu9250Register.FIFOEnable, value); }
        }

        /// <summary>
        /// Gets or sets the I2C master control configuration register 36.
        /// </summary>
        public byte Register36
        {
            get { return Hardware.WriteReadByte((byte)Mpu9250Register.I2cMasterControl); }
            set { Hardware.WriteJoinByte((byte)Mpu9250Register.I2cMasterControl, value); }
        }

        /// <summary>
        /// Gets or sets I2C master status configuration register 54.
        /// </summary>
        public byte Register54
        {
            get { return Hardware.WriteReadByte((byte)Mpu9250Register.I2cMasterStatus); }
            set { Hardware.WriteJoinByte((byte)Mpu9250Register.I2cMasterStatus, value); }
        }

        /// <summary>
        /// Gets or sets the INT pin/bypass enable configuration register 55.
        /// </summary>
        public byte Register55
        {
            get { return Hardware.WriteReadByte((byte)Mpu9250Register.INTPinConfig); }
            set { Hardware.WriteJoinByte((byte)Mpu9250Register.INTPinConfig, value); }
        }

        /// <summary>
        /// Gets or sets the interrupt enable configuration register 56.
        /// </summary>
        public byte Register56
        {
            get { return Hardware.WriteReadByte((byte)Mpu9250Register.INTEnable); }
            set { Hardware.WriteJoinByte((byte)Mpu9250Register.INTEnable, value); }
        }

        /// <summary>
        /// Gets or sets the interrupt status configuration register 58.
        /// </summary>
        public byte Register58
        {
            get { return Hardware.WriteReadByte((byte)Mpu9250Register.INTStatus); }
            set { Hardware.WriteJoinByte((byte)Mpu9250Register.INTStatus, value); }
        }

        /// <summary>
        /// Gets or sets I2C master delay control configuration register 103.
        /// </summary>
        public byte Register103
        {
            get { return Hardware.WriteReadByte((byte)Mpu9250Register.I2cMasterDelayControl); }
            set { Hardware.WriteJoinByte((byte)Mpu9250Register.I2cMasterDelayControl, value); }
        }

        /// <summary>
        /// Gets or sets signal path reset configuration register 104.
        /// </summary>
        public byte Register104
        {
            get { return Hardware.WriteReadByte((byte)Mpu9250Register.SignalPathReset); }
            set { Hardware.WriteJoinByte((byte)Mpu9250Register.SignalPathReset, value); }
        }

        /// <summary>
        /// Gets or sets accelerometer interrupt control configuration register 105.
        /// </summary>
        public byte Register105
        {
            get { return Hardware.WriteReadByte((byte)Mpu9250Register.MOTDetectControl); }
            set { Hardware.WriteJoinByte((byte)Mpu9250Register.MOTDetectControl, value); }
        }

        /// <summary>
        /// Gets or sets user control configuration register 106.
        /// </summary>
        public byte Register106
        {
            get { return Hardware.WriteReadByte((byte)Mpu9250Register.UserControl); }
            set { Hardware.WriteJoinByte((byte)Mpu9250Register.UserControl, value); }
        }

        /// <summary>
        /// Gets or sets power management 1 configuration register 107.
        /// </summary>
        public byte Register107
        {
            get { return Hardware.WriteReadByte((byte)Mpu9250Register.PowerManagment1); }
            set { Hardware.WriteJoinByte((byte)Mpu9250Register.PowerManagment1, value); }
        }

        /// <summary>
        /// Gets or sets power management 1 configuration register 108.
        /// </summary>
        public byte Register108
        {
            get { return Hardware.WriteReadByte((byte)Mpu9250Register.PowerManagment2); }
            set { Hardware.WriteJoinByte((byte)Mpu9250Register.PowerManagment2, value); }
        }

        /// <summary>
        /// Gets or sets FIFO high count configuration register 114.
        /// </summary>
        public byte Register114
        {
            get { return Hardware.WriteReadByte((byte)Mpu9250Register.FIFOCountHigh); }
            set { Hardware.WriteJoinByte((byte)Mpu9250Register.FIFOCountHigh, value); }
        }

        /// <summary>
        /// Gets or sets FIFO low count configuration register 115.
        /// </summary>
        public byte Register115
        {
            get { return Hardware.WriteReadByte((byte)Mpu9250Register.FIFOCountLow); }
            set { Hardware.WriteJoinByte((byte)Mpu9250Register.FIFOCountLow, value); }
        }

        /// <summary>
        /// Gets or sets FIFO read/write configuration register 116.
        /// </summary>
        public byte Register116
        {
            get { return Hardware.WriteReadByte((byte)Mpu9250Register.FIFOReadWrite); }
            set { Hardware.WriteJoinByte((byte)Mpu9250Register.FIFOReadWrite, value); }
        }

        #endregion

        #endregion

        #region Protected Properties

        /// <summary>
        /// SPI device.
        /// </summary>
        [CLSCompliant(false)]
        protected SpiDevice Hardware { get; private set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Resets the device and clears <see cref="SensorReading"/>.
        /// </summary>
        public virtual void Reset(bool ak8963 = true)
        {

            //  Triggers the reset bit for the MPU9250 and AK8963 chip.
            if (ak8963 == true)
            {
                WriteAK8963(Mpu9250MagRegister.MagControl2, 0x1);
                Task.Delay(ResetDelay);
            }

            Hardware.WriteJoinByte((byte)Mpu9250Register.PowerManagment1, 0x80);
            Task.Delay(ResetDelay);

            // Clear result
            SensorReading = Mpu9250SensorReading.Zero;
        }

        /// <summary>
        /// Accumulates gyro and accelerometer data after device initialization. It calculates the average
        /// of the at-rest readings and then loads the resulting offsets into accelerometer and gyro bias registers.
        /// </summary>
        public virtual void Calibrate()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Runs accelerometer and gyroscope self test. Checks the calibration agents the factory settings. A percent
        /// of deviation from factory trim values less then +/- 14 is a pass.
        /// </summary>
        /// <returns></returns>
        public virtual double[] SelfTest()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Reads sensor data from the device then updates <see cref="SensorReading"/> property.
        /// </summary>
        public virtual void Update()
        {
            // Read all sensor values
            var sensorResult = ReadAll();

            // Fire reading changed event
            OnReadingChanged?.Invoke(this, null); 

            // Update properties
            SensorReading = sensorResult;
        }

        /// <summary>
        /// Reads all sensor data from the device.
        /// </summary>
        public virtual Mpu9250SensorReading ReadAll()
        {
            // Read sensor values
            var results = ReadMotion9();

            SensorReading.AccelXAxis = results[0];
            SensorReading.AccelYAxis = results[1];
            SensorReading.AccelZAxis = results[2];
            SensorReading.GyroXAxis = results[3];
            SensorReading.GyroYAxis = results[4];
            SensorReading.GyroZAxis = results[5];
            SensorReading.MagXAxis = results[6];
            SensorReading.MagYAxis = results[7];
            SensorReading.MagZAxis = results[8];
   
            // Update fusion results
            if (OperationMode == Mpu9250OperationsMode.Fusion ||
                    OperationMode == Mpu9250OperationsMode.Fusion2)
                SensorReading.Update();

            // Return results
            return SensorReading;
        }

        /// <summary>
        /// Reads accelerometer, gyroscope and magnetometer data from the device.
        /// </summary>
        /// <returns>
        /// Accelerometer: x/y/z
        /// Gyroscope: x/y/z
        /// Magnetometer : x/y/z
        /// </returns>
        public virtual double[] ReadMotion9()
        {
            // Set I2C slave address of the AK8963 and set for read.
            Hardware.WriteJoinByte((byte)Mpu9250Register.I2cSlave0Address, (byte)Mpu9250MagRegister.Ak8963I2cAddress | SpiExtensions.ReadFlag);

            // Set I2C slave 0 register address from where to begin data transfer.
            Hardware.WriteJoinByte((byte)Mpu9250Register.I2cSlave0Register, (byte)Mpu9250MagRegister.MagDataXHigh);

            // Read 7 bytes from the magnetometer
            Hardware.WriteJoinByte((byte)Mpu9250Register.I2cSlave0Control, 0x87);

            // Wait for write completion
            Task.Delay(WriteDelay).Wait();

            // Read the data out registers
            var bytes = Hardware.WriteReadBytes((byte)Mpu9250Register.AccelDataXHigh, 21);

            var sensorReading = new double[9]
            {
                ((short)(bytes[0] << 8 | bytes[1]) - _axisOffset.AccelXAxisOffset) * _accelMuliplier,
                ((short)(bytes[2] << 8 | bytes[3]) - _axisOffset.AccelYAxisOffset) * _accelMuliplier,
                ((short)(bytes[4] << 8 | bytes[5]) - _axisOffset.AccelZAxisOffset) * _accelMuliplier,
                ((short)(bytes[8] << 8 | bytes[9]) - _axisOffset.GyroXAxisOffset) * _gyroMuliplier,
                // skipping temperature register
                ((short)(bytes[10] << 8 | bytes[11]) - _axisOffset.GyroYAxisOffset) * _gyroMuliplier,
                ((short)(bytes[12] << 8 | bytes[13]) - _axisOffset.GyroZAxisOffset) * _gyroMuliplier,
                (short)(bytes[15] << 8 | bytes[14]) * _magMuliplier * _axisOffset.MagXAxisSensitivity - _axisOffset.MagXAxisOffset,
                (short)(bytes[17] << 8 | bytes[16]) * _magMuliplier * _axisOffset.MagYAxisSensitivity - _axisOffset.MagYAxisOffset,
                (short)(bytes[19] << 8 | bytes[18]) * _magMuliplier * _axisOffset.MagZAxisSensitivity - _axisOffset.MagZAxisOffset
            };
           
            // Return results
            return sensorReading;
        }

        /// <summary>
        /// Reads accelerometer and gyroscope data from the device.
        /// </summary>
        /// <returns>
        /// Accelerometer: x/y/z
        /// Gyroscope: x/y/z
        /// </returns>
        public virtual double[] ReadMotion6()
        {
            // Read register
            var bytes = Hardware.WriteReadBytes((byte)Mpu9250Register.AccelDataXHigh, 6);

            var sensorReading = new double[6]
            {
                (short)(bytes[0] << 8 | bytes[1]) * _accelMuliplier - _axisOffset.AccelXAxisOffset,
                (short)(bytes[2] << 8 | bytes[3]) * _accelMuliplier - _axisOffset.AccelYAxisOffset,
                (short)(bytes[4] << 8 | bytes[5]) * _accelMuliplier - _axisOffset.AccelZAxisOffset,
                (short)(bytes[6] << 8 | bytes[7]) * _gyroMuliplier - _axisOffset.GyroXAxisOffset,
                (short)(bytes[8] << 8 | bytes[9]) * _gyroMuliplier - _axisOffset.GyroYAxisOffset,
                (short)(bytes[10] << 8 | bytes[11]) * _gyroMuliplier - _axisOffset.GyroZAxisOffset
            };

            // Return results
            return sensorReading;
        }

        /// <summary>
        /// Reads the MPU-9250 die temperature in celsius.
        /// </summary>
        /// <returns>Temperature</returns>
        public virtual double ReadTemperature()
        {
            // Read register and update property 
            var sensorReading = new short[1]
            {
                Hardware.WriteReadByte((byte)Mpu9250Register.TemperatureDataHigh)
            };

            // Return results
            return (((sensorReading[0] - RoomTemperatureOffset) / TempSensitivity) + 21);
        }

        /// <summary>
        /// Reads raw accelerometer data from the device.
        /// </summary>
        /// <returns>Accelerometer: x/y/z</returns>
        public virtual short[] ReadAccelerometer()
        {
            // Read register
            var bytes = Hardware.WriteReadBytes((byte)Mpu9250Register.AccelDataXHigh, 6);

            var sensorReading = new short[3]
            {
                (short)(bytes[0] << 8 | bytes[1]),
                (short)(bytes[2] << 8 | bytes[3]),
                (short)(bytes[4] << 8 | bytes[5])
            };

            // Return results
            return sensorReading;
        }

        /// <summary>
        /// Reads raw gyroscope data from the device.
        /// </summary>
        /// <returns>Gyroscope: x/y/z</returns>
        public virtual short[] ReadGyroscope()
        {
            // Read register
            var bytes = Hardware.WriteReadBytes((byte)Mpu9250Register.GyroDataXHigh, 6);

            short[] sensorReading = new short[3]
            {
                (short)(bytes[0] << 8 | bytes[1]),
                (short)(bytes[2] << 8 | bytes[3]),
                (short)(bytes[4] << 8 | bytes[5])
            };

            // Return results
            return sensorReading;
        }

        /// <summary>
        /// Reads raw magnetometer data from the device. Measurement range of each
        /// axis is from -32760 to 32760 in 16-bit output.
        /// </summary>
        public virtual short[] ReadMagnetometer()
        {
            // Read register
            var bytes = ReadAK8963(Mpu9250MagRegister.MagDataXHigh, 6);

            var sensorReading = new short[3]
            {
                (short)(bytes[1] << 8 | bytes[0]),
                (short)(bytes[3] << 8 | bytes[2]),
                (short)(bytes[5] << 8 | bytes[4])
            };

            // Return results
            return sensorReading;
        }

        /// <summary>
        /// Reads raw sensor data from the device then updates <see cref="AxisOffset"/> property.
        /// </summary>
        public virtual Mpu9250OffsetReading ReadOffset()
        {
            Mpu9250OffsetReading axisOffset = new Mpu9250OffsetReading();

            // Read the data out registers
            var accelBytes = Hardware.WriteReadBytes((byte)Mpu9250Register.OffsetXAHigh, 6);

            axisOffset.AccelXAxisOffset = (short)(accelBytes[1] << 8 | accelBytes[0]);
            axisOffset.AccelYAxisOffset = (short)(accelBytes[3] << 8 | accelBytes[2]);
            axisOffset.AccelZAxisOffset = (short)(accelBytes[5] << 8 | accelBytes[4]);

            // Read the data out registers
            var gyroBytes = Hardware.WriteReadBytes((byte)Mpu9250Register.OffsetXGHigh, 6);

            axisOffset.GyroXAxisOffset = (short)(gyroBytes[1] << 8 | gyroBytes[0]);
            axisOffset.GyroYAxisOffset = (short)(gyroBytes[3] << 8 | gyroBytes[2]);
            axisOffset.GyroZAxisOffset = (short)(gyroBytes[5] << 8 | gyroBytes[4]);

            // Read the data out registers
            var magBytes = ReadMagSensitivity();

            axisOffset.MagXAxisSensitivity = magBytes[0];
            axisOffset.MagYAxisSensitivity = magBytes[1];
            axisOffset.MagZAxisSensitivity = magBytes[2];

            _axisOffset = axisOffset;

            // Return results
            return axisOffset;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Read raw magnetometer sensitivity data from the fuse ROM.
        /// </summary>
        /// <returns>x/y/z</returns>
        private double[] ReadMagSensitivity()
        {
            double[] sensitivity = new double[3] { 0, 0, 0 };

            // Power down magnetometer
            WriteAK8963(Mpu9250MagRegister.MagControl1, 0x00);

            // Enter fuse ROM access mode
            WriteAK8963(Mpu9250MagRegister.MagControl1, 0x0F);

            // Read the data out registers
            var magBytes = ReadAK8963(Mpu9250Register.ExternalSensorData00, Mpu9250MagRegister.MagXSensitivity, 0x83, 3);

            sensitivity[0] = (float)(magBytes[0] - 128) / 256.0 + 1.0;
            sensitivity[1] = (float)(magBytes[1] - 128) / 256.0 + 1.0;
            sensitivity[2] = (float)(magBytes[2] - 128) / 256.0 + 1.0;

            return sensitivity;
        }

        #region AK8963 Registers Access

        /// <summary>
        /// Sets or clears one or more bits on the AK8963 magnetometer.
        /// </summary>
        /// <param name="register">Magnetometer register</param>
        /// <param name="mask"> Mask of the bit to set or clear according to value. Supports setting or clearing multiple bits.</param>
        /// <param name="value">Value of the bits, i.e. set or clear.</param>
        /// <returns>Value written to register</returns>
        /// <remarks>
        /// Commonly used to set register flags. Reads the current byte value, merges the positive or negative bit mask according to value,
        /// then writes the modified byte back.
        /// </remarks>
        private byte ReadWriteAK8963(Mpu9250MagRegister register, byte mask, bool value)
        {
            // Read existing byte
            var oldByte = ReadAK8963(register);

            // Merge bit (set or clear bit accordingly)
            var newByte = value ? (byte)(oldByte | mask) : (byte)(oldByte & ~mask);

            // Write new byte
            WriteAK8963(register, newByte);

            // Return the value written.
            return newByte;
        }

        /// <summary>
        /// Reads a single byte result from the AK8963 magnetometer register.
        /// </summary>
        /// <param name="register">Magnetometer register.</param>
        /// <returns>Read register byte.</returns>
        private byte ReadAK8963(Mpu9250MagRegister register)
        {
            return ReadAK8963(register, 1)[0];
        }

        /// <summary>
        /// Reads one or more bytes from the AK8963 magnetometer register.
        /// </summary>
        /// <param name="register">Magnetometer register.</param>
        /// <param name="size">Amount of bytes to read.</param>
        /// <returns>Read register bytes.</returns>
        private byte[] ReadAK8963(Mpu9250MagRegister register, int size)
        {

            // Read the data out registers
            return ReadAK8963(Mpu9250Register.ExternalSensorData00, register, (byte)((sbyte)size | 0x80), size);
        }

        /// <summary>
        /// Reads one or more bytes from the AK8963 magnetometer register.
        /// </summary>
        /// <param name="register">External sensor data to read register.</param>
        /// <param name="magRegister">Magnetometer register.</param>
        /// <param name="control">Magnetometer register.</param>
        /// <param name="size">Amount of bytes to read.</param>
        /// <returns>Read register bytes.</returns>
        private byte[] ReadAK8963(Mpu9250Register register, Mpu9250MagRegister magRegister, byte control, int size)
        {
            // Validate size to make sure it's not out of the range for an sbyte type.
            if (size < -128 & size > 127) throw new ArgumentOutOfRangeException(nameof(size));

            // Set I2C slave address of the AK8963 and set for read.
            Hardware.WriteJoinByte((byte)Mpu9250Register.I2cSlave0Address, (byte)Mpu9250MagRegister.Ak8963I2cAddress | SpiExtensions.ReadFlag);

            // Set I2C slave 0 register address from where to begin data transfer.
            Hardware.WriteJoinByte((byte)Mpu9250Register.I2cSlave0Register, (byte)magRegister);

            // Read bytes from the magnetometer
            Hardware.WriteJoinByte((byte)Mpu9250Register.I2cSlave0Control, control);

            // Wait for write completion
            Task.Delay(WriteDelay).Wait();

            // Read the data out registers
            return Hardware.WriteReadBytes((byte)register, size);
        }

        /// <summary>
        /// Writes a single byte to the AK8963 magnetometer register.
        /// </summary>
        /// <param name="register">Magnetometer register.</param>
        /// <param name="data">Data to write.</param>
        private void WriteAK8963(Mpu9250MagRegister register, byte data)
        {
            // Set I2C slave address of the AK8963 and set for write.
            Hardware.WriteJoinByte((byte)Mpu9250Register.I2cSlave0Address, (byte)Mpu9250MagRegister.Ak8963I2cAddress);

            // Set I2C slave register address from where to begin data transfer.
            Hardware.WriteJoinByte((byte)Mpu9250Register.I2cSlave0Register, (byte)register);

            // Set I2C data written to I2C slave.
            Hardware.WriteJoinByte((byte)Mpu9250Register.I2cSlave0DO, data);

            // Set I2C enable and set 1 byte.
            Hardware.WriteJoinByte((byte)Mpu9250Register.I2cSlave0Control, 0x81);

            // Wait for write completion
            Task.Delay(WriteDelay).Wait();
        }

        #endregion

        #endregion

        #region Event Handlers

        /// <summary>
        /// Event handler for the <see cref="Update"/> method.
        /// </summary>
        public EventHandler<Mpu9250Device> OnReadingChanged;

        /// <summary>
        /// Method triggered when the criteria for motion detection is fulfilled and the motion interrupt is generated.
        /// </summary>
        public EventHandler<Mpu9250Device> OnMotionDetected;

        #endregion

    }
}
