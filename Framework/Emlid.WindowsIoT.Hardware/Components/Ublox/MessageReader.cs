using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Spi;

namespace Emlid.WindowsIot.Hardware.Components.Ublox
{
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class MessageReader
    {
        #region Constants

        /// <summary>
        /// Time to wait before polling the device in milliseconds
        /// </summary>
        public const int PollingDelay = 75;

        /// <summary>
        /// Time to wait after reading the device in milliseconds.
        /// </summary>
        public const int ReadDelay = 0;

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
        /// 
        /// </summary>
        public MessageReader(SpiDevice device)
        {
            Hardware = device;

            _messageFactory = new MessageFactory();
            _parserMessage = new MemoryStream();
            _nmeaState = NMEAState.Start;
            _ubxState = UBXState.Start;
        }

        #endregion 

        #region Private Fields

        /// <summary>
        /// 
        /// </summary>
        private CancellationTokenSource _parserToken;

        /// <summary>
        /// 
        /// </summary>
        private Task _parserTask;

        /// <summary>
        /// 
        /// </summary>
        private MemoryStream _parserMessage;

        /// <summary>
        /// 
        /// </summary>
        private MessageFactory _messageFactory;

        /// <summary>
        /// 
        /// </summary>
        private enum NMEAState
        {
            Start, Body, ChecksumA, ChecksumB, CR, LF, End
        };

        /// <summary>
        /// 
        /// </summary>
        private NMEAState _nmeaState;

        /// <summary>
        /// 
        /// </summary>
        private byte _nmeaChecksum = 0x00;
        
        /// <summary>
        /// 
        /// </summary>
        private byte _nmeaCrc = 0x00;


        /// <summary>
        /// 
        /// </summary>
        private enum UBXState
        {
            Start, Sync2, Class, ID, Length1, Length2, Payload, ChecksumA, ChecksumB, End
        };

        /// <summary>
        /// 
        /// </summary>
        private UBXState _ubxState;

        /// <summary>
        /// 
        /// </summary>
        private ushort _ubxLength = 0;

        /// <summary>
        /// 
        /// </summary>
        private byte _ubxCrcA = 0x00;

        /// <summary>
        /// 
        /// </summary>
        private byte _ubxCrcB = 0x00;

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

        #endregion

        #region Private Methods

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

        #endregion  
    }
}
