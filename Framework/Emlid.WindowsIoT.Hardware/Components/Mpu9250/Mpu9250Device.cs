using Emlid.WindowsIot.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Windows.Devices.I2c;
using Windows.Devices.Spi;

using Emlid.WindowsIot.Hardware.Components;
using Windows.Foundation;
using System.Diagnostics;
using Windows.Devices.Gpio;
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
        /// Time to wait for the <see cref="Sleep"/> and <see cref="Wake"/> command to complete in millisecond.
        /// </summary>
        public const int SleepModeDelay = 50;

        /// <summary>
        /// Time to wait for the device to startup in millisecond.
        /// </summary>
        public const int StartupDelay = 400;

        /// <summary>
        /// Time to wait after a write for the device to update registers in millisecond.
        /// </summary>
        public const int WriteDelay = 50;


        /// <summary>
        /// The standard acceleration due to gravity in m/s2.
        /// </summary>
        public const double StandardGravity = 9.80665;

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
            SensorReading = new Mpu9250SensorReading();

            // Verify the device is connected, accessable and ready for use.
            if (ChipId == 0x71 & Ak8963ChipId == 0x48)
                IsConnected = true;

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
        /// GPIO Interrupt enable pin.
        /// </summary>
        private GpioPin _interruptPin;

        /// <summary>
        ///  Accelerometer scale divide. 
        /// </summary>
        private double _accelDivider = 1;

        /// <summary>
        ///  Gyroscope scale divide. 
        /// </summary>
        private double _gyroDivider = 1;

        /// <summary>
        ///  Gyroscope scale divide. 
        /// </summary>
        private double _magDivider = 1;

        /// <summary>
        ///  Gyroscope scale divide. 
        /// </summary>
        private Mpu9250AxisOffsetReading _axisOffset = new Mpu9250AxisOffsetReading();

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
            get
            {
                //// Set I2C master mode.
                //Hardware.WriteJoinByte((byte)Mpu9250Register.UserControl, 0x20);

                //// Set I2C configuration multi-master I2C 400KHz.
                //Hardware.WriteJoinByte((byte)Mpu9250Register.I2cMasterControl, 0x0D);

                //// Set I2C slave address of the AK8963 and set for write.
                Hardware.WriteJoinByte((byte)Mpu9250Register.I2cSlave0Address, (byte)Mpu9250Register.Ak8963I2cAddress | SpiExtensions.ReadFlag);

                //// Set I2C slave 0 register address from where to begin data transfer.
                Hardware.WriteJoinByte((byte)Mpu9250Register.I2cSlave0Register, 0x00);

                //// Set to read 1 bytes from the magnetometer.
                Hardware.WriteJoinByte((byte)Mpu9250Register.I2cSlave0Control, 0x81);

                // Write delay
                Task.Delay(WriteDelay);

                Debug.WriteLine(Hardware.WriteReadByte((byte)Mpu9250Register.ExternalSensorData00));

                return Hardware.WriteReadByte((byte)Mpu9250Register.ExternalSensorData00);
            }
        }

        /// <summary>
        /// Gets or sets the GPIO pin which enables the motion interrupt of the device.
        /// </summary>
        public int MotionEnableGpioPin { get; set; }

        /// <summary>
        /// Enables or disables the motion <see cref="MotionEnableGpioPin"/> for the device.
        /// </summary>
        public bool MotionEnabled
        {
            get
            {
                return _interruptPin.GetDriveMode() == GpioPinDriveMode.Input;
            }
            set
            {
                if (value == true)
                {

                    // Set interrupt bypass
                    Hardware.WriteJoinByte((byte)Mpu9250Register.ConfigINTPin, 0x30);

                    _interruptPin = GpioController.GetDefault().OpenPin(MotionEnableGpioPin);

                    //Ignore changes in value of less than 50ms
                    _interruptPin.DebounceTimeout = new TimeSpan(0, 0, 0, 0, 50);
                    _interruptPin.SetDriveMode(GpioPinDriveMode.Input);
                    //_interruptEnablePin.ValueChanged += OnMotionDetected;
                }
                else
                {
                    // Clear interrupt bypass
                    Hardware.WriteJoinByte((byte)Mpu9250Register.ConfigINTPin, 0x00);

                    _interruptPin = null;
                }
            }
        }

        /// <summary>
        /// Enables or disables sensor fusion.
        /// </summary>
        public bool FusionEnabled { get; set; }

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
        /// Gets or sets accelerometer, gyroscope and magnetometer axis offset of the device.
        /// </summary>>
        public Mpu9250AxisOffsetReading AxisOffset
        {
            get { return ReadAxisOffset(); }
            set { WriteAxisOffset(value); }
        }
        /// <summary>
        /// Gets or sets the scale range for the gyroscope.
        /// </summary>
        public Mpu9250ConfigGyroScale GyroScale
        {
            get
            {
                return (Mpu9250ConfigGyroScale)Hardware.WriteReadByte((byte)Mpu9250Register.ConfigGyro);
            }

            protected set
            {
                // Write to register
                Hardware.WriteJoinByte((byte)Mpu9250Register.ConfigGyro, (byte)GyroScale);

                switch (GyroScale)
                {
                    case Mpu9250ConfigGyroScale.Scale250Dbps:
                        _gyroDivider = 131;
                        break;
                    case Mpu9250ConfigGyroScale.Scale500Dbps:
                        _gyroDivider = 65.5;
                        break;
                    case Mpu9250ConfigGyroScale.Scale1000Dbps:
                        _gyroDivider = 32.8;
                        break;
                    case Mpu9250ConfigGyroScale.Scale2000Dbps:
                        _gyroDivider = 16.4;
                        break;
                }
            }
        }

        /// <summary>
        /// Gets or sets the scale range for the accelerometer.
        /// </summary>
        public Mpu9250ConfigAccelScale AccelScale
        {
            get
            {
                return (Mpu9250ConfigAccelScale)Hardware.WriteReadByte((byte)Mpu9250Register.ConfigAccel);
            }

            protected set
            {
                // Write to register
                Hardware.WriteJoinByte((byte)Mpu9250Register.ConfigAccel, (byte)AccelScale);

                switch (AccelScale)
                {
                    case Mpu9250ConfigAccelScale.Scale2G:
                        _accelDivider = 16384;
                        break;
                    case Mpu9250ConfigAccelScale.Scale4G:
                        _accelDivider = 8192;
                        break;
                    case Mpu9250ConfigAccelScale.Scale8G:
                        _accelDivider = 4096;
                        break;
                    case Mpu9250ConfigAccelScale.Scale16G:
                        _accelDivider = 2048;
                        break;
                }
            }
        }

        /// <summary>
        /// Result of the last <see cref="Update"/>.
        /// </summary>>
        public Mpu9250SensorReading SensorReading { get; protected set; }

        #region Register Configuration

        /// <summary>
        /// Gets or sets if the device configuration register 26.
        /// </summary>
        public byte Config
        {
            get { return Hardware.WriteReadByte((byte)Mpu9250Register.Config); }
            set { Hardware.WriteJoinByte((byte)Mpu9250Register.Config, value); }
        }

        /// <summary>
        /// Gets or sets if the device accelerometer configuration register.
        /// </summary>
        public byte AccelConfig1
        {
            get { return Hardware.WriteReadByte((byte)Mpu9250Register.ConfigAccel); }
            set { Hardware.WriteJoinByte((byte)Mpu9250Register.ConfigAccel, value); }
        }

        /// <summary>
        /// Gets or sets if the device accelerometer configuration register.
        /// </summary>
        public byte AccelConfig2
        {
            get { return Hardware.WriteReadByte((byte)Mpu9250Register.ConfigAccel2); }
            set { Hardware.WriteJoinByte((byte)Mpu9250Register.ConfigAccel2, value); }
        }

        /// <summary>
        /// Gets or sets if the device user control configuration register.
        /// </summary>
        public byte UserConfig
        {
            get { return Hardware.WriteReadByte((byte)Mpu9250Register.UserControl); }
            set { Hardware.WriteJoinByte((byte)Mpu9250Register.UserControl, value); }
        }

        /// <summary>
        /// Gets or sets if the device I2C master control configuration register.
        /// </summary>
        public byte I2cMasterConfig
        {
            get { return Hardware.WriteReadByte((byte)Mpu9250Register.I2cMasterControl); }
            set { Hardware.WriteJoinByte((byte)Mpu9250Register.I2cMasterControl, value); }
        }

        /// <summary>
        /// Gets or sets the device power management 1 device configuration register 107 settings.  
        /// See <see cref="Mpu9250ClockSourceBits"/>, <see cref="Mpu9250SleepModeBits"/> and <see cref="Mpu9250ResetSystemBits"/>  for additional details.
        /// </summary>
        /// <example>
        /// <code>
        /// Power1Config = (byte)Mpu9250ClockSourceBits.AutoSelect | (byte)Mpu9250SleepModeBits.Wake;
        /// </code>
        /// </example>
        public byte Power1Config
        {
            get { return Hardware.WriteReadByte((byte)Mpu9250Register.PowerManagment1); }
            set { Hardware.WriteJoinByte((byte)Mpu9250Register.PowerManagment1, value); }
        }

        /// <summary>
        /// Gets or sets if the power management 2 device register.
        /// </summary>
        public byte Power2Config
        {
            get { return Hardware.WriteReadByte((byte)Mpu9250Register.PowerManagment2); }
            set { Hardware.WriteJoinByte((byte)Mpu9250Register.PowerManagment2, value); }
        }

        /// <summary>
        /// Gets or sets if the sample rate divider device register.
        /// </summary>
        public byte Rate
        {
            get { return Hardware.WriteReadByte((byte)Mpu9250Register.SampleRateDiv); }
            set { Hardware.WriteJoinByte((byte)Mpu9250Register.SampleRateDiv, value); }
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

        #region Sleep

        /// <summary>
        /// Enters sleep mode.
        /// </summary>
        /// <remarks>
        /// Sets the <see cref="Mpu9250Register.PowerManagment1"/> register <see cref="Mpu9250SleepModeBits.Sleep"/> bit
        /// then waits for <see cref="ResetDelay"/> to allow the registers to stabilize.
        /// </remarks>
        /// <returns>
        /// True when mode was changed, false when already set.
        /// </returns>
        public virtual bool Sleep()
        {
            // Read sleep bit (do nothing when already sleeping)
            if ((Mpu9250SleepModeBits)Power1Config == Mpu9250SleepModeBits.Sleep)
                return false;

            // Set sleep bit
            Power1Config = (byte)Mpu9250SleepModeBits.Sleep;

            // Wait for completion
            Task.Delay(SleepModeDelay).Wait();

            // Return changed
            return true;

        }

        /// <summary>
        /// Leaves sleep mode.
        /// </summary>
        /// <remarks>
        /// Clears the <see cref="Mpu9250Register.PowerManagment1"/> register <see cref="Mpu9250SleepModeBits.Wake"/> bit
        /// then waits for <see cref="ResetDelay"/> to allow the oscillator to start.
        /// </remarks>
        /// <returns>
        /// True when mode was changed, false when not sleeping.
        /// </returns>
        public virtual bool Wake()
        {
            // Read sleep bit (do nothing when already sleeping)
            if ((Mpu9250SleepModeBits)Power1Config == Mpu9250SleepModeBits.Wake)
                return false;

            // Clear sleep bit
            Power1Config = (byte)Mpu9250SleepModeBits.Wake;

            // Wait for completion
            Task.Delay(SleepModeDelay).Wait();

            // Return changed
            return true;
        }

        #endregion

        /// <summary>
        /// Resets the device and clears <see cref="Mpu9250ResetSystemBits.Enable"/>.
        /// </summary>
        public virtual void Reset()
        {
            // Set register bit
            Power1Config = (byte)Mpu9250ResetSystemBits.Enable;

            // Wait for completion
            Task.Delay(ResetDelay).Wait();

            // Clear result
            SensorReading = Mpu9250SensorReading.Zero;
        }

        /// <summary>
        /// Resets the device and clears <see cref="Mpu9250ResetSystemBits.Enable"/>.
        /// </summary>
        public virtual void ResetMagnetometer()
        {
            // Set I2C slave 0 register address from where to begin data transfer.
            Hardware.WriteJoinByte((byte)Mpu9250Register.I2cSlave0Register, 0x0B);

            // Set I2C reset trigger
            Hardware.WriteJoinByte((byte)Mpu9250Register.I2cSlave0DO, 0x1);

            // Set I2C enable and set 1 byte
            Hardware.WriteJoinByte((byte)Mpu9250Register.I2cSlave0Control, 0x81);

            // Clear result
            SensorReading = Mpu9250SensorReading.Zero;
        }

        /// <summary>
        /// Reads sensor data from the device then updates <see cref="SensorReading"/> property.
        /// </summary>
        public virtual void Update()
        {
            // Read all sensor values
            var sensorResult = ReadAll();

            // Fire reading changed event
            if (ReadingChanged != null)
            {
                //ReadingChanged(this, new Mpu9250ReadingChangedEventArgs(sensorResult));
            }

            // Update properties
            SensorReading = sensorResult;
        }

        /// <summary>
        /// Reads all sensor data from the device.
        /// </summary>
        public virtual Mpu9250SensorReading ReadAll()
        {

            // Set I2C slave address of the AK8963 and set for read.
            Hardware.WriteJoinByte((byte)Mpu9250Register.I2cSlave0Address, (byte)Mpu9250Register.Ak8963I2cAddress | SpiExtensions.ReadFlag);

            // Set I2C slave 0 register address from where to begin data transfer.
            Hardware.WriteJoinByte((byte)Mpu9250Register.I2cSlave0Register, 0x03);

            // Read 7 bytes from the magnetometer
            Hardware.WriteJoinByte((byte)Mpu9250Register.I2cSlave0Control, 0x87);

            // Read the data out registers
            var bytes = Hardware.WriteReadBytes((byte)Mpu9250Register.AccelDataXHigh, 21);

            Mpu9250SensorReading sensorReading = new Mpu9250SensorReading()
            {

                AccelXAxis = (float)((bytes[0] << 8 | bytes[1]) - _axisOffset.AccelXAxisOffset / _accelDivider),
                AccelYAxis = (float)((bytes[2] << 8 | bytes[3]) - _axisOffset.AccelYAxisOffset / _accelDivider),
                AccelZAxis = (float)((bytes[4] << 8 | bytes[5]) - _axisOffset.AccelZAxisOffset / _accelDivider),
                Temperature = (float)(((bytes[6] << 8 | bytes[7]) / 340) + 36.53),
                GyroXAxis = (float)((bytes[8] << 8 | bytes[9]) - _axisOffset.GyroXAxisOffset / _gyroDivider),
                GyroYAxis = (float)((bytes[10] << 8 | bytes[11]) - _axisOffset.GyroYAxisOffset / _gyroDivider),
                GyroZAxis = (float)((bytes[12] << 8 | bytes[13]) - _axisOffset.GyroZAxisOffset / _gyroDivider),
                MagXAxis = (float)((bytes[15] << 8 | bytes[14]) - _axisOffset.MagXAxisOffset / _magDivider),
                MagYAxis = (float)((bytes[17] << 8 | bytes[16]) - _axisOffset.MagYAxisOffset / _magDivider),
                MagZAxis = (float)((bytes[19] << 8 | bytes[18]) - _axisOffset.MagZAxisOffset / _magDivider)
            };

            // Update fusion results
            if (FusionEnabled == true)
                sensorReading.Update();

            // Return results
            return sensorReading;
        }

        /// <summary>
        /// Reads accelerometer data from the device.
        /// </summary>
        public virtual Mpu9250SensorReading ReadAccelerometer()
        {
            // Read the data out registers
            var bytes = Hardware.WriteReadBytes((byte)Mpu9250Register.AccelDataXHigh, 6);

            var sensorReading = new Mpu9250SensorReading()
            {
                AccelXAxis = (float)((bytes[0] << 8 | bytes[1]) - _axisOffset.AccelXAxisOffset / _accelDivider),
                AccelYAxis = (float)((bytes[2] << 8 | bytes[3]) - _axisOffset.AccelYAxisOffset / _accelDivider),
                AccelZAxis = (float)((bytes[4] << 8 | bytes[5]) - _axisOffset.AccelZAxisOffset / _accelDivider)
            };

            // Return results
            return sensorReading;
        }

        /// <summary>
        /// Reads gyroscope data from the device.
        /// </summary>
        public virtual Mpu9250SensorReading ReadGyroscope()
        {
            // Read the data out registers
            var bytes = Hardware.WriteReadBytes((byte)Mpu9250Register.GyroDataXHigh, 6);

            var sensorReading = new Mpu9250SensorReading()
            {
                GyroXAxis = (float)((bytes[0] << 8 | bytes[1]) - _axisOffset.GyroXAxisOffset / _gyroDivider),
                GyroYAxis = (float)((bytes[2] << 8 | bytes[3]) - _axisOffset.GyroXAxisOffset / _gyroDivider),
                GyroZAxis = (float)((bytes[4] << 8 | bytes[5]) - _axisOffset.GyroXAxisOffset / _gyroDivider)
            };

            // Return results
            return sensorReading;
        }

        /// <summary>
        /// Reads magnetometer data from the device.
        /// </summary>
        public virtual Mpu9250SensorReading ReadMagnetometer()
        {
            // Set I2C slave address of the AK8963 and set for read.
            Hardware.WriteJoinByte((byte)Mpu9250Register.I2cSlave0Address, (byte)Mpu9250Register.Ak8963I2cAddress | SpiExtensions.ReadFlag);

            // Set I2C slave 0 register address from where to begin data transfer.
            Hardware.WriteJoinByte((byte)Mpu9250Register.I2cSlave0Register, 0x03);

            // Read 7 bytes from the magnetometer
            Hardware.WriteJoinByte((byte)Mpu9250Register.I2cSlave0Control, 0x87);

            // Read the data out registers
            var bytes = Hardware.WriteReadBytes((byte)Mpu9250Register.ExternalSensorData00, 7);

            var sensorReading = new Mpu9250SensorReading()
            {
                MagXAxis = (float)((bytes[0] << 8 | bytes[1]) - _axisOffset.MagXAxisOffset / _magDivider),
                MagYAxis = (float)((bytes[2] << 8 | bytes[3]) - _axisOffset.MagYAxisOffset / _magDivider),
                MagZAxis = (float)((bytes[4] << 8 | bytes[5]) - _axisOffset.MagZAxisOffset / _magDivider)
            };

            // Return results
            return sensorReading;
        }

        /// <summary>
        /// Reads Mpu9250 die temperature data from the device.
        /// </summary>
        public virtual Mpu9250SensorReading ReadTemperature()
        {

            // Read the data out registers
            var bytes = Hardware.WriteReadBytes((byte)Mpu9250Register.TemperatureDataHigh, 2);

            var sensorReading = new Mpu9250SensorReading()
            { Temperature = (float)(((bytes[0] << 8 | bytes[1]) / 340) + 36.53) };

            // Return results
            return sensorReading;
        }

        /// <summary>
        /// This is a test device initilization process.
        /// </summary>
        public virtual void Init()
        {
            //Set clock source
            //Hardware.WriteJoinByte((byte)Mpu9250Register.PowerManagment1, 0x01);
            Power1Config = (byte)Mpu9250ClockSourceBits.AutoSelect | (byte)Mpu9250SleepModeBits.Wake;

            // Enable accelerometer and gyroscope sensor
            Hardware.WriteJoinByte((byte)Mpu9250Register.PowerManagment2, 0x00);

            // Set gyroscope bandwidth 184Hz, and temprature bandwidth 188Hz.
            Hardware.WriteJoinByte((byte)Mpu9250Register.Config, 0x00);

            // Set gyroscope scale to +-2000dps
            Hardware.WriteJoinByte((byte)Mpu9250Register.ConfigGyro, 0x18);

            // Set accelerometer scale to +-16G
            Hardware.WriteJoinByte((byte)Mpu9250Register.ConfigAccel, 3 << 3)
;
            // Set accelerometer data reates, enable accel LPF, and bandwidth 184Hz
            Hardware.WriteJoinByte((byte)Mpu9250Register.ConfigAccel2, 0x08);

            // Set interrupt bypass
            Hardware.WriteJoinByte((byte)Mpu9250Register.ConfigINTPin, 0x30);

            // Set I2C master mode
            Hardware.WriteJoinByte((byte)Mpu9250Register.UserControl, 0x20);

            // Set I2C configureation multi-master IIC 400KHz.
            Hardware.WriteJoinByte((byte)Mpu9250Register.I2cMasterControl, 0x0D);


            // Set I2C slave address of the AK8963 and set for write.
            Hardware.WriteJoinByte((byte)Mpu9250Register.I2cSlave0Address, (byte)Mpu9250Register.Ak8963I2cAddress);


            //////  RESET MAG
            // Set I2C slave 0 register address from where to begin data transfer.
            //Hardware.WriteJoinByte((byte)Mpu9250Register.I2cSlave0Register, 0x0B);

            // Set I2C reset trigger
            //Hardware.WriteJoinByte((byte)Mpu9250Register.I2cSlave0DO, 0x1);

            // Set I2C enable and set 1 byte
            //Hardware.WriteJoinByte((byte)Mpu9250Register.I2cSlave0Control, 0x81);


            ////// CONTINUOUS MEASUREMENT
            // Set I2C slave 0 register address from where to begin data transfer.
            Hardware.WriteJoinByte((byte)Mpu9250Register.I2cSlave0Register, 0x0A);

            // Set I2C reset trigger
            Hardware.WriteJoinByte((byte)Mpu9250Register.I2cSlave0DO, 0x12);

            // Set I2C enable and set 1 byte
            Hardware.WriteJoinByte((byte)Mpu9250Register.I2cSlave0Control, 0x81);


        }

        #endregion

        #region Private Methods

        private void InitMpu9250()
        {
            //Set clock source
            Hardware.WriteJoinByte((byte)Mpu9250Register.PowerManagment1, 0x01);

            // Enable accelerometer and gyroscope sensor
            Hardware.WriteJoinByte((byte)Mpu9250Register.PowerManagment2, 0x00);

            // Set gyroscope bandwidth 184Hz, and temprature bandwidth 188Hz.
            Hardware.WriteJoinByte((byte)Mpu9250Register.Config, 0x00);

            // Set gyroscope scale to +-2000dps
            GyroScale = Mpu9250ConfigGyroScale.Scale2000Dbps;

            // Set accelerometer scale to +-16G
            AccelScale = Mpu9250ConfigAccelScale.Scale16G;

            // Set accelerometer data reates, enable accel LPF, and bandwidth 184Hz
            Hardware.WriteJoinByte((byte)Mpu9250Register.ConfigAccel2, 0x08);

            // Set I2C master mode
            Hardware.WriteJoinByte((byte)Mpu9250Register.UserControl, 0x20);

            // Set I2C configureation multi-master IIC 400KHz.
            Hardware.WriteJoinByte((byte)Mpu9250Register.I2cMasterControl, 0x0D);

            // Set I2C slave 0 register address from where to begin data transfer.
            Hardware.WriteJoinByte((byte)Mpu9250Register.I2cSlave0Register, 0x0A);

            // Set to continuous mesurement in 16bit
            Hardware.WriteJoinByte((byte)Mpu9250Register.I2cSlave0DO, 0x12);

            // Set I2C enable and set 1 byte
            Hardware.WriteJoinByte((byte)Mpu9250Register.I2cSlave0Control, 0x81);

        }

        private void InitAk8963()
        {


        }

        private void InitContinuousMesurement()
        {

            // Set I2C slave address of the AK8963 and set for write.
            Hardware.WriteJoinByte((byte)Mpu9250Register.I2cSlave0Address, (byte)Mpu9250Register.Ak8963I2cAddress);

            // Set I2C slave 0 register address from where to begin data transfer.
            Hardware.WriteJoinByte((byte)Mpu9250Register.I2cSlave0Register, 0x0A);

            // Set to continuous mesurement in 16bit
            Hardware.WriteJoinByte((byte)Mpu9250Register.I2cSlave0DO, 0x12);

            // Set I2C enable and set 1 byte
            Hardware.WriteJoinByte((byte)Mpu9250Register.I2cSlave0Control, 0x81);
        }

        /// <summary>
        /// Reads sensor data from the device then updates <see cref="AxisOffset"/> property.
        /// </summary>
        private Mpu9250AxisOffsetReading ReadAxisOffset()
        {
            Mpu9250AxisOffsetReading axisOffset = new Mpu9250AxisOffsetReading();

            // Read the data out registers
            var bytes = Hardware.WriteReadBytes((byte)Mpu9250Register.OffsetXAHigh, 6);

            axisOffset.AccelXAxisOffset = (short)(bytes[1] << 8 | bytes[0]);
            axisOffset.AccelYAxisOffset = (short)(bytes[3] << 8 | bytes[2]);
            axisOffset.AccelZAxisOffset = (short)(bytes[5] << 8 | bytes[4]);

            // Read the data out registers
            bytes = Hardware.WriteReadBytes((byte)Mpu9250Register.OffsetYGHigh, 6);

            axisOffset.GyroXAxisOffset = (short)(bytes[1] << 8 | bytes[0]);
            axisOffset.GyroYAxisOffset = (short)(bytes[3] << 8 | bytes[2]);
            axisOffset.GyroZAxisOffset = (short)(bytes[5] << 8 | bytes[4]);

            // Set I2C slave address of the AK8963 and set for read.
            Hardware.WriteJoinByte((byte)Mpu9250Register.I2cSlave0Address, (byte)Mpu9250Register.Ak8963I2cAddress | SpiExtensions.ReadFlag);

            // Set I2C slave 0 register address from where to begin data transfer.
            Hardware.WriteJoinByte((byte)Mpu9250Register.I2cSlave0Register, 0x03);

            // Read 7 bytes from the magnetometer
            Hardware.WriteJoinByte((byte)Mpu9250Register.I2cSlave0Control, 0x87);

            // Read the data out registers
            bytes = Hardware.WriteReadBytes((byte)Mpu9250Register.ExternalSensorData00, 7);

            axisOffset.MagXAxisOffset = (short)(bytes[1] << 8 | bytes[0]);
            axisOffset.MagYAxisOffset = (short)(bytes[3] << 8 | bytes[2]);
            axisOffset.MagZAxisOffset = (short)(bytes[5] << 8 | bytes[4]);

            _axisOffset = axisOffset;

            // Return results
            return axisOffset;
        }

        /// <summary>
        /// Write <see cref="AxisOffset"/> property to the device.
        /// </summary>
        private void WriteAxisOffset(Mpu9250AxisOffsetReading axisOffset)
        {
            //TODO:
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public double GetPitch(double x, double z)
        {
            return (Math.Atan2(x, z) + Math.PI) * (180 / Math.PI) - 180;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public double GetRoll(double y, double z)
        {
            return (Math.Atan2(y, z) + Math.PI) * (180 / Math.PI) - 180;
        }


        /// <summary>
        /// Algorithm object.
        /// </summary>
        static MadgwickAHRS Ahrs = new MadgwickAHRS(1f / 256f, 0.1f);
        //AHRS.Update(deg2rad(e.Gyroscope[0]), deg2rad(e.Gyroscope[1]), deg2rad(e.Gyroscope[2]), e.Accelerometer[0], e.Accelerometer[1], e.Accelerometer[2], e.Magnetometer[0], e.Magnetometer[1], e.Magnetometer[2]);        

        /// <summary>
        /// Converts degrees to radians.
        /// </summary>
        /// <param name="degrees">
        /// Angular quantity in degrees.
        /// </param>
        /// <returns>
        /// Angular quantity in radians.
        /// </returns>
        static float deg2rad(float degrees)
        {
            return (float)(Math.PI / 180) * degrees;
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Event handler for the <see cref="Update"/> method.
        /// </summary>
        public EventHandler<Mpu9250Device> ReadingChanged;

        /// <summary>
        /// Method triggered when the criteria for motion detection is fulfilled and the motion interrupt is generated.
        /// </summary>
        public EventHandler<Mpu9250Device> MotionDetected;

        #endregion

    }
}
