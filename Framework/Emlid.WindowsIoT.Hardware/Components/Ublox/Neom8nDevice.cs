using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Collections.Generic;

using Windows.Devices.Spi;

using Emlid.WindowsIot.Common;
using Emlid.WindowsIot.Hardware.Components.Ublox.Ubx;


namespace Emlid.WindowsIot.Hardware.Components.Ublox
{
    /// <summary>
    /// NEO-M8N high performance GPS positioning sensor.
    /// </summary>
    public class Neom8nDevice : DisposableObject
    {
        #region Constants

        /// <summary>
        /// Time to wait for the command to complete in milliseconds.
        /// </summary>
        public const int ResetDelay = 10000;

        /// <summary>
        /// Time to wait for the device to startup in milliseconds.
        /// </summary>
        public const int StartupDelay = 100;

        /// <summary>
        /// Time to wait after a write for the device to update registers in milliseconds.
        /// </summary>
        public const int WriteDelay = 10;

        /// <summary>
        /// Time to wait before timing out waiting for the acknowledgement response in seconds
        /// </summary>
        public const int AcknowledgementTimeOut = 1;

        /// <summary>
        /// Maximum message size of the ublox NEO-M8N receiver.  
        /// </summary>
        public const int MaximumMessageSize = 1024;

        /// <summary>
        /// Mininum message size of the ublox NEO-M8N receiver.  
        /// </summary>
        public const int MininumMessageSize = 8;

        #endregion

        #region Lifetime

        /// <summary>
        /// Creates an instance using the specified device and sampling rate.
        /// </summary>
        /// <param name="device">SPI device.</param>
        [CLSCompliant(false)]
        public Neom8nDevice(SpiDevice device)
        {
            // Validate
            if (device == null) throw new ArgumentNullException(nameof(device));

            // Initialize hardware
            Hardware = device;

            // Initialize message reader
            Reader = new MessageReader(device);

            // Initialize members
            Acknowlagements = new Dictionary<int, DateTime>();
            GeodeticSensorReading = new GeodeticSensorReading();

        }

        /// <summary>
        /// <see cref="DisposableObject.Dispose(bool)"/>.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            // Only managed resources to dispose
            if (!disposing)
                return;

            // Stop message polling
            StopPolling();

            // Close device
            Hardware?.Dispose();
        }

        #endregion

        #region Private Fields

        #endregion

        #region Public Properties

        /// <summary>
        /// 
        /// </summary>
        public event EventHandler<MessageReceivedEventArgs> MessageReceived
        {
            add { Reader.MessageReceived += value; }
            remove { Reader.MessageReceived -= value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public Dictionary<int, DateTime> Acknowlagements { get; protected set; }

        /// <summary>
        /// Geodetic sensor reading result from the last polling.
        /// </summary>>
        public GeodeticSensorReading GeodeticSensorReading { get; protected set; }

        #endregion

        #region Protected Properties

        /// <summary>
        /// SPI device.
        /// </summary>
        [CLSCompliant(false)]
        protected SpiDevice Hardware { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        [CLSCompliant(false)]
        protected MessageReader Reader { get; private set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// 
        /// </summary>
        public void ReadVersion()
        {
            // Create message
            var versions = new ReceiverSoftware();

            // Write message to receiver
            WriteMessage(versions);
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual bool SetPollingRate(byte classId, byte messageId, byte rate)
        {
            // Create message
            var message = new PollingMessageRate();
            message.Class = classId;
            message.SubClass = messageId;
            message.Rate = rate;

            // Write message to receiver
            return WriteMessage(message);

        }

        /// <summary>
        /// Resets the device and clears sensors readings.
        /// </summary>
        public virtual void Reset(ResetMode mode = ResetMode.SoftwareReset)
        {
            // Create message
            var message = new ResetReceiver();
            message.ResetMode = mode;

            // Write message to receiver
            WriteMessage(message);

            // Clear result
            GeodeticSensorReading = GeodeticSensorReading.Zero;

            // Wait for reset completion
            Task.Delay(ResetDelay).Wait();
        }

        /// <summary>
        /// 
        /// </summary>
        public void StartPolling()
        {
            Reader.Start();
        }

        /// <summary>
        /// 
        /// </summary>
        public void StopPolling()
        {
            Reader.Stop();
        }

        /// <summary>
        /// Write message to the ublox reciver.
        /// </summary>
        /// <param name="message"></param>
        public bool WriteMessage(IMessageBase message)
        {
            byte[] messageArray = message.ToArray();

            WriteReceiver(messageArray);

            if (message.IsAcknowledged)
            {
                int acknowledgeKey = new { Class = 0x05, Id = 0x01, MessageClass = messageArray[2], MessageId = messageArray[3] }.GetHashCode();

                var stopwatch = new Stopwatch();
                stopwatch.Start();

                do
                {
                    if (Acknowlagements.ContainsKey(acknowledgeKey))
                    {
                        Acknowlagements.Remove(acknowledgeKey);
                        return true;
                    }
                    else
                    {
                        Task.Delay(10).Wait();
                    }

                } while (stopwatch.Elapsed < TimeSpan.FromSeconds(AcknowledgementTimeOut));
            }

            return false;
        }

        /// <summary>
        /// Write bytes to ublox receiver
        /// </summary>
        /// <param name="array"></param>
        public void WriteReceiver(byte[] array)
        {
            if (array.Length == 0) return;

            // Write to receiver
            Hardware.Write(array);

            // Wait for completion
            Task.Delay(WriteDelay).Wait();
        }

        #endregion

    }
}