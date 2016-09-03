using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Windows.Devices.I2c;
using Windows.Devices.Enumeration;
using System.Globalization;
using Emlid.WindowsIot.Hardware.Boards.Navio;

using Windows.Devices.Spi;
using Emlid.WindowsIot.Hardware.Components;
using Emlid.WindowsIot.Hardware.Components.Mpu9250;

namespace Emlid.WindowsIot.Hardware.Boards.Navio
{
    /// <summary>
    /// Navio 9-axis motion tracking device (MPU9250 hardware device).
    /// </summary>
    public class NavioMotionDevice : Mpu9250Device
    {

        #region Constants

        /// <summary>
        /// SPI controller index of the MPU9250 chip on the Navio2 board.
        /// </summary>
        public const int SpiControllerIndex = 0;

        /// <summary>
        /// SPI chip select line of the MPU9250 chip on the Navio2 board.
        /// </summary>
        public const int ChipSelect = 1;

        /// <summary>
        /// SPI operating frequency of the MPU9250 chip on the Navio2 board.
        /// </summary>
        public const int Frequency = 1000000;

        /// <summary>
        /// SPI data length in bits of the MPU9250 chip on the Navio2 board.
        /// </summary>
        public const int DataLength = 8;

        #endregion

        #region Lifetime

        /// <summary>
        /// Creates an instance with initialization.
        /// </summary>

        [CLSCompliant(false)]
        public NavioMotionDevice()
            : base(NavioHardwareProvider.ConnectSpi(SpiControllerIndex, ChipSelect, Frequency, DataLength, SpiMode.Mode3))
        {}

        /// <summary>
        /// Creates an initialized instance of the device configured with fusion and the default settings.
        /// </summary>
        public static NavioMotionDevice Initialize(Mpu9250Placement placement, bool reset = true)
        {
            // Create device
            var device = new NavioMotionDevice();

            // Test Init()
            device.Init();

            // Return initialized device
            return device;

            //// Reset device to its default configuration.
            //if (reset)
            //   device.Reset();

            //// Set device placement to properly set x/y/z axis.
            //if (placement != Mpu9250Placement.P1)
            //    device.Placement = placement;

            //// Set device to use a navio clock source.
            //device.Power1Config = 0x01;

            //// Enable accelerometer and gyroscope sensor
            //device.Power2Config = 0x00;

            //// Set gyroscope bandwidth 184Hz, and temprature bandwidth 188Hz.
            //device.Config = 0x00;

            //// Set accelerometer data reates, enable accel LPF, and bandwidth 184Hz
            //device.AccelConfig2 = 0x08;

            //// Set gyroscope scale to +-2000dps
            ////device.GyroScale = Mpu9250ConfigGyroScale.Scale2000Dbps;

            //// Set accelerometer scale to +-16G
            ////device.AccelScale = Mpu9250ConfigAccelScale.Scale16G;

            //// Set I2C master mode
            //device.UserConfig = 0x20;

            //// Set I2C configureation multi-master IIC 400KHz.
            //device.I2cMasterConfig = 0x0D;

            //device.ResetMagnetometer();


        }

        #endregion
    }
}
