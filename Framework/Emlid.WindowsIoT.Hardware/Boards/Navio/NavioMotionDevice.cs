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
        /// GPIO interrupt pin of the MPU9250 chip on the Navio2 board.
        /// </summary>
        public const int MotionEnableGpioPin = 23;

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
        public static NavioMotionDevice Initialize(Mpu9250Placement placement = Mpu9250Placement.P1, Mpu9250OperationsMode mode = Mpu9250OperationsMode.Fusion)
        {
            // Create device
            var device = new NavioMotionDevice();

            // Set placement
            device.Placement = placement;

            // Set fusion mode
            device.OperationMode = mode;
            
            // Read factory offsets
            device.ReadOffset();

            // Return initialized device
            return device;
        }

        #endregion
    }
}
