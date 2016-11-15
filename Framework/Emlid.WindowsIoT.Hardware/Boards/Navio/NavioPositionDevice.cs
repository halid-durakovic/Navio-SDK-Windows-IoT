using System;

using Windows.Devices.Spi;
using Emlid.WindowsIot.Hardware.Components.Ublox;

namespace Emlid.WindowsIot.Hardware.Boards.Navio
{
    /// <summary>
    /// Navio high performance GPS positioning device (NEO-M8N hardware device).
    /// </summary>
    public class NavioPositionDevice : Neom8nDevice
    {
        #region Constants

        /// <summary>
        /// SPI controller index of the NEO-M8N chip on the Navio2 board.
        /// </summary>
        public const int SpiControllerIndex = 0;

        /// <summary>
        /// SPI chip select line of the NEO-M8N chip on the Navio2 board.
        /// </summary>
        public const int ChipSelect = 0;

        /// <summary>
        /// SPI operating frequency of the NEO-M8N chip on the Navio2 board.
        /// </summary>
        public const int Frequency = 5500000; // 1000000

        /// <summary>
        /// SPI data length in bits of the NEO-M8N chip on the Navio2 board.
        /// </summary>
        public const int DataLength = 8;

        #endregion

        #region Lifetime

        /// <summary>
        /// Creates an instance with initialization.
        /// </summary>

        [CLSCompliant(false)]
        public NavioPositionDevice()
            : base(NavioHardwareProvider.ConnectSpi(SpiControllerIndex, ChipSelect, Frequency, DataLength, SpiMode.Mode0))
        { }

        /// <summary>
        /// Creates an initialized instance of the device configured with fusion and the default settings.
        /// </summary>
        public static NavioPositionDevice Initialize()
        {
            // Create device
            var device = new NavioPositionDevice();

            // Return initialized device
            return device;
        }

        #endregion
    }
}
