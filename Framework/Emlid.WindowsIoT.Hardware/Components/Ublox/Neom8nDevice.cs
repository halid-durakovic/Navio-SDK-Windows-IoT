using System;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

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
        /// Time to wait after reading the device in milliseconds.
        /// </summary>
        public const int ReadDelay = 0;

        /// <summary>
        /// Time to wait before polling the device in milliseconds
        /// </summary>
        public const int PollingDelay = 75;

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

            _messageFactory = new MessageFactory();
            _parserMessage = new MemoryStream();
            _nmeaState = NMEAState.Start;
            _ubxState = UBXState.Start;
            GeodeticSensorReading = new GeodeticSensorReading();

            // Initialize message received handler 
            MessageReceived += OnMessageReceived;

            // Initialize message polling
            Start();

            // Initialize port configuration
            //var portConfig = new PortConfiguration();
            //portConfig.IsInUbx = true;
            //portConfig.IsOutUbx = true;
            //portConfig.Mode[9] = true;
            //portConfig.Mode[12] = true;
            //portConfig.Mode[13] = true;
            //WriteMessage(portConfig);
            //Task.Delay(1000).Wait();

            // Initialize software and hardware versions
            //var versions = new ReceiverSoftware();
            //WriteMessage(versions);
            //Task.Delay(1000).Wait();

            // Initialize polling for messages
            //SetPollingRate(0x01, 0x02, 0x01);
            //Task.Delay(1000).Wait();

            // Loop until first message is verified
            //do
            //{
            //    Task.Delay(1000).Wait();

            //} while (IsConnected == false);

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
            Stop();

            // Close device
            Hardware?.Dispose();
        }

        #endregion

        #region Private Fields


        private CancellationTokenSource _parserToken;

        private Task _parserTask;

        private MemoryStream _parserMessage;

        private MessageFactory _messageFactory;

        private enum NMEAState
        {
            Start, Body, ChecksumA, ChecksumB, CR, LF, End
        };
        private NMEAState _nmeaState;
        private byte _nmeaChecksum = 0x00;
        private byte _nmeaCrc = 0x00;

        private enum UBXState
        {
            Start, Sync2, Class, ID, Length1, Length2, Payload, ChecksumA, ChecksumB, End
        };
        private UBXState _ubxState;
        private ushort _ubxLength = 0;
        private byte _ubxCrcA = 0x00;
        private byte _ubxCrcB = 0x00;

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

        #endregion

        #region Public Methods

        /// <summary>
        /// 
        /// </summary>
        public virtual void SetPollingRate(byte classId, byte messageId, byte rate )
        {
            // Create message
            var message = new PollingMessageRate();
            message.Class = classId;
            message.SubClass = messageId;
            message.Rate = rate;

            // Write message to receiver
            WriteMessage(message);

            // Wait for reset completion
            Task.Delay(ResetDelay).Wait();

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
        public void Start()
        {
            _parserToken = new CancellationTokenSource();
            _parserTask = Task.Run(() =>
            {
                while (true)
                {
                    MessageParser();
                }
            }, _parserToken.Token);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Stop()
        {
            _parserToken.Cancel();
            _parserTask.Wait();
        }

        /// <summary>
        /// Write message to the ublox reciver.
        /// </summary>
        /// <param name="message"></param>
        public void WriteMessage(IMessageBase message)
        {
            WriteReceiver(message.ToArray());
        }

        /// <summary>
        /// Read bytes from ublox receiver.
        /// </summary>
        /// <param name="size"></param>
        public byte[] ReadReceiver(int size)
        {
            byte[] readBuffer = new byte[size];

            // Read from receiver
            Hardware.Read(readBuffer);

            // Wait for completion
            Task.Delay(ReadDelay).Wait();

            return readBuffer;
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

        #region Private Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnMessageReceived(object sender, MessageReceivedEventArgs args)
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
                // Mark connected true 
                IsConnected = true;

            }
            else if (args.MessageType == typeof(ReceiverSoftware))
            {
                ReceiverSoftware message = (ReceiverSoftware)args.MessageResult;
                SoftwareVersion = Encoding.ASCII.GetString(message.SoftwareVersion);
                HardwareVersion = Encoding.ASCII.GetString(message.HardwareVersion);

                // Mark connected true 
                IsConnected = true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void MessageParser()
        {
            foreach (byte receiverByte in ReadReceiver(MininumMessageSize))
            {
                if (_ubxState != UBXState.Start)
                {
                    ParseUBX(receiverByte);
                }
                else if (receiverByte == 0xB5)
                {
                    _ubxState = UBXState.Start;
                    ParseUBX(receiverByte);
                }
                else if (_nmeaState != NMEAState.Start)
                {
                    ParseNMEA(receiverByte);
                }
                else if (receiverByte == '$')
                {
                    _nmeaState = NMEAState.Start;
                    ParseNMEA(receiverByte);
                }
                else
                {
                    // Delay if bytes (0xFF) are being disguarded
                    Task.Delay(PollingDelay);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="recevierByte"></param>
        private void ParseNMEA(byte recevierByte)
        {
            switch (_nmeaState)
            {
                case NMEAState.Start:
                    _parserMessage.SetLength(0);
                    _nmeaChecksum = 0;
                    _nmeaCrc = 0;
                    _nmeaState = NMEAState.Body;
                    break;

                case NMEAState.Body:
                    if (recevierByte == '*')
                    {
                        _nmeaState = NMEAState.ChecksumA;
                    }
                    else if (recevierByte == '\r')
                    {
                        Debug.WriteLine("NMEA Message Resetting: Body terminated without checksum");
                        _nmeaState = NMEAState.LF;
                    }
                    else
                    {
                        _nmeaChecksum ^= recevierByte;
                    }
                    break;

                case NMEAState.ChecksumA:
                    _nmeaCrc = (byte)(ByteToHex(recevierByte) << 4);
                    _nmeaState = NMEAState.ChecksumB;
                    break;

                case NMEAState.ChecksumB:
                    _nmeaCrc |= ByteToHex(recevierByte);
                    if (_nmeaChecksum == _nmeaCrc)
                    {
                        _nmeaState = NMEAState.CR;
                    }
                    else
                    {
                        Debug.WriteLine("NMEA Message Resetting: Checksum failed");
                        _nmeaState = NMEAState.Start;
                        return;
                    }
                    break;

                case NMEAState.CR:
                    if (recevierByte == '\r')
                    {
                        _nmeaState = NMEAState.LF;
                    }
                    else
                    {
                        Debug.WriteLine("NMEA Message Resetting: CR failed");
                        _nmeaState = NMEAState.Start;
                        return;
                    }
                    break;

                case NMEAState.LF:
                    if (recevierByte == '\n')
                    {
                        _nmeaState = NMEAState.End;
                    }
                    else
                    {
                        Debug.WriteLine("NMEA Message Resetting: LF failed");
                        _nmeaState = NMEAState.Start;
                        return;
                    }
                    break;
            }

            _parserMessage.WriteByte(recevierByte);

            if (_nmeaState == NMEAState.End)
            {
                // Sending the received message after getting the last line feed 
                MessageReceived?.Invoke(this, new MessageReceivedEventArgs(_parserMessage.ToArray(), null, MessageProtocol.NEMA));
                _nmeaState = NMEAState.Start;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="receiverByte"></param>
        private void ParseUBX(byte receiverByte)
        {
            switch (_ubxState)
            {
                case UBXState.Start:
                    _parserMessage.SetLength(0);
                    _ubxCrcA = 0x00;
                    _ubxCrcB = 0x00;
                    _ubxState = UBXState.Sync2;
                    break;

                case UBXState.Sync2:
                    if (receiverByte == 0x62)
                    {
                        _ubxState = UBXState.Class;
                    }
                    else
                    {
                        Debug.WriteLine("UBX Message Resetting: Sync2 failed");
                        _ubxState = UBXState.Start;
                        return;
                    }
                    break;

                case UBXState.Class:
                    _ubxCrcA += receiverByte;
                    _ubxCrcB += _ubxCrcA;
                    _ubxState = UBXState.ID;
                    break;

                case UBXState.ID:
                    _ubxCrcA += receiverByte;
                    _ubxCrcB += _ubxCrcA;
                    _ubxState = UBXState.Length1;
                    break;

                case UBXState.Length1:
                    _ubxCrcA += receiverByte;
                    _ubxCrcB += _ubxCrcA;
                    _ubxLength = receiverByte;
                    _ubxState = UBXState.Length2;
                    break;

                case UBXState.Length2:
                    _ubxCrcA += receiverByte;
                    _ubxCrcB += _ubxCrcA;
                    _ubxLength += (ushort)(receiverByte << 8);
                    _ubxState = UBXState.Payload;
                    break;

                case UBXState.Payload:
                    _ubxCrcA += receiverByte;
                    _ubxCrcB += _ubxCrcA;
                    if (_ubxLength + 5 == _parserMessage.Position)
                    {
                        _ubxState = UBXState.ChecksumA;
                    }
                    if (_parserMessage.Position >= 1022)
                    {
                        Debug.WriteLine("UBX Message Resetting: Payload is too large");
                        _ubxState = UBXState.Start;
                        return;
                    }
                    break;

                case UBXState.ChecksumA:
                    if (_ubxCrcA == receiverByte)
                    {
                        _ubxState = UBXState.ChecksumB;
                    }
                    else
                    {
                        Debug.WriteLine("UBX Message Resetting: Checksum A failed");
                        _ubxState = UBXState.Start;
                        return;
                    }
                    break;

                case UBXState.ChecksumB:
                    if (_ubxCrcB == receiverByte)
                    {
                        _ubxState = UBXState.End;
                    }
                    else
                    {
                        Debug.WriteLine("UBX Message Resetting: Checksum B failed");
                        _ubxState = UBXState.Start;
                        return;
                    }
                    break;
            }

            _parserMessage.WriteByte(receiverByte);

            if (_ubxState == UBXState.End)
            {
                // Sending the received message after getting the last checksum

                byte[] message = _parserMessage.ToArray();
                IMessageResult messageObject = _messageFactory.Invoke(message);

                messageObject.TryParse(message);

                MessageReceived?.Invoke(this, new MessageReceivedEventArgs(message, messageObject, MessageProtocol.UBX));
                _ubxState = UBXState.Start;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="receiverByte"></param>
        /// <returns></returns>
        private byte ByteToHex(byte receiverByte)
        {
            if (receiverByte <= '9' && receiverByte >= '0')
            {
                return (byte)(receiverByte - '0');
            }
            else if (receiverByte >= 'A' && receiverByte <= 'F')
            {
                return (byte)(receiverByte - 'A' + 10);
            }
            return 0x00;
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Event handler fired when a new polling message is available.
        /// </summary>
        public event EventHandler<MessageReceivedEventArgs> MessageReceived;

        /// <summary>
        /// Event handler fired when sensor reading changed.
        /// </summary>
        public event EventHandler<GeodeticSensorReading> GeodeticSensorChanged;

        #endregion

    }
}