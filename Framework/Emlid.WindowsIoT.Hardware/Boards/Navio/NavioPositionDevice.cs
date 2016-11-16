using System;
using System.Text;
using Windows.Devices.Spi;

using Emlid.WindowsIot.Hardware.Components.Ublox;
using Emlid.WindowsIot.Hardware.Components.Ublox.Ubx;


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
        {

            // Initialize message received handler 
            Reader.MessageReceived += OnMessageReceived;
        }

        /// <summary>
        /// Creates an initialized instance of the device configured with fusion and the default settings.
        /// </summary>
        public static NavioPositionDevice Initialize()
        {
            // Create device
            var device = new NavioPositionDevice();

            // Initialize message polling
            device.StartPolling();

            // Initialize port configuration
            var portConfig = new PortConfiguration();
            portConfig.IsInUbx = true;
            portConfig.IsOutUbx = true;
            portConfig.Mode[9] = true;
            portConfig.Mode[12] = true;
            portConfig.Mode[13] = true;
            var portResponse = device.WriteMessage(portConfig);

            // Read current software/hardware version from receiver
            device.ReadVersion();

            // Initialize polling for messages
            device.SetPollingRate(0x01, 0x02, 0x01);

            // Return initialized device
            return device;

        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Indicates if the positioning sensor is connected, accessable and ready for use.
        /// </summary>
        public bool IsConnected { get; protected set; }

        /// <summary>
        /// 
        /// </summary>
        public string SoftwareVersion { get; protected set; }

        /// <summary>
        /// 
        /// </summary>
        public string HardwareVersion { get; protected set; }

        #endregion

        #region Private Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnMessageReceived(object sender, MessageReceivedEventArgs args)
        {
            // Mark connected true 
            if (!IsConnected)
                IsConnected = true;

            //Debug.WriteLine(ArrayExtensions.HexDump(args.Message));

            if (args.MessageProtocol == MessageProtocol.UBX)
            {
                if (args.MessageType == typeof(GeodeticPosition))
                {
                    GeodeticPosition message = (GeodeticPosition)args.MessageResult;

                    GeodeticSensorReading.Latitude = message.Latitude;
                    GeodeticSensorReading.Longitude = message.Longitude;
                    GeodeticSensorReading.TimeMillisOfWeek = (int)message.TimeMillisOfWeek;
                    GeodeticSensorReading.VerticalAccuracy = message.VerticalAccuracy;
                    GeodeticSensorReading.HorizontalAccuracy = message.HorizontalAccuracy;
                    GeodeticSensorReading.HeightAboveEllipsoid = message.HeightAboveEllipsoid;
                    GeodeticSensorReading.HeightAboveSeaLevel = message.HeightAboveSeaLevel;

                    // Fire reading changed event
                    GeodeticSensorChanged?.Invoke(this, GeodeticSensorReading);
                }
                else if (args.MessageType == typeof(Acknowledge))
                {

                    int key = new { Class = args.Message[2], Id = args.Message[3], MessageClass = args.Message[6], MessageId = args.Message[7] }.GetHashCode();

                    // Add acknowlagement
                    Acknowlagements.Add(key, DateTime.Now);

                }
                else if (args.MessageType == typeof(NotAcknowledge))
                {
                    //int key = new { Class = args.Message[2], Id = args.Message[3], MessageClass = args.Message[6], MessageId = args.Message[7] }.GetHashCode();

                    //// Add not acknowlagement
                    //_acknowlagements.Add(key, DateTime.Now);

                }
                else if (args.MessageType == typeof(ReceiverSoftware))
                {
                    ReceiverSoftware message = (ReceiverSoftware)args.MessageResult;
                    SoftwareVersion = Encoding.ASCII.GetString(message.SoftwareVersion);
                    HardwareVersion = Encoding.ASCII.GetString(message.HardwareVersion);
                }
            }
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Event handler fired when sensor reading changed.
        /// </summary>
        public event EventHandler<GeodeticSensorReading> GeodeticSensorChanged;

        #endregion
    }
}
