using System;
using Windows.Devices.Spi;
using Emlid.WindowsIot.Common;

namespace Emlid.WindowsIot.Hardware.Components.Neo
{
    /// <summary>
    /// NEO-M8N high performance GPS positioning sensor.
    /// </summary>
    public class NeoM8nDevice : DisposableObject
    {
        #region Constants

        #endregion

        #region Lifetime

        /// <summary>
        /// Creates an instance using the specified device and sampling rate.
        /// </summary>
        /// <param name="device">SPI device.</param>
        [CLSCompliant(false)]
        public NeoM8nDevice(SpiDevice device) 
        {
            // Validate
            if (device == null) throw new ArgumentNullException(nameof(device));

            // Initialize hardware
            Hardware = device;
 
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

        #endregion

        #region Public Properties

        /// <summary>
        /// Indicates if the positioning  sensor is connected, accessable and ready for use.
        /// </summary>
        public bool IsConnected { get; protected set; }

        #endregion

        #region Protected Properties

        /// <summary>
        /// SPI device.
        /// </summary>
        [CLSCompliant(false)]
        protected SpiDevice Hardware { get; private set; }

        #endregion

        #region Public Methods

        #endregion

        #region Private Methods

        #endregion

        #region Event Handlers

        #endregion

    }
}