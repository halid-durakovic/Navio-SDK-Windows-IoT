using System;
using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Devices.Spi;
using Windows.Storage.Streams;
using Windows.Networking.Sockets;

using Emlid.WindowsIot.Common;
using Emlid.WindowsIot.Hardware.Boards.Navio;
using Emlid.WindowsIot.Hardware.Components.Ublox;

namespace Emlid.WindowsIot.NavioUbloxService
{
    /// <summary>
    /// Navio high performance GPS positioning device service for Ublox u-center.
    /// </summary>
    public class NavioUcenterDevice : Neom8nDevice
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
        public const int Frequency = 5500000;

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
        public NavioUcenterDevice()
            : base(NavioHardwareProvider.ConnectSpi(SpiControllerIndex, ChipSelect, Frequency, DataLength, SpiMode.Mode0))
        {
            ActiveClients = new Dictionary<Guid, StreamSocket>();
            MessageReceived += (sender, args) => OnMessageReceived(sender, args);
        }

        /// <summary>
        /// Creates an initialized instance of the device configured with fusion and the default settings.
        /// </summary>
        public static NavioUcenterDevice Initialize()
        {
            // Create device
            var device = new NavioUcenterDevice();

            // Return initialized device
            return device;
        }

        #endregion


        #region Private Fields

        private StreamSocketListener _listener;
        private int _servicePort = 0;

        #endregion

        #region Public Properties

        /// <summary>
        /// 
        /// </summary>
        [CLSCompliant(false)]
        public Dictionary<Guid, StreamSocket> ActiveClients { get; set; }

        /// <summary>
        ///  Gets the port for receiving data
        /// </summary>
        public int ActivePort
        {
            get
            {
                if (_listener == null)
                    return -1;

                return (int.Parse(_listener.Information.LocalPort));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsRunning
        {
            get { return (_listener != null); }
        }

        #endregion

        /// <summary>
        /// Starts the service listener and messsage poller.
        /// </summary>
        /// <param name="servicePort">The port used to listen on. You can use port <c>0</c> to let the OS assign a port. </param>
        public async void StartService(int servicePort)
        {
            if (_listener != null)
            {
                StopService();
            }

            if (_listener == null)
            {

                try
                {
                    StartPolling();

                    _servicePort = servicePort;

                    _listener = new StreamSocketListener();
                    _listener.Control.KeepAlive = true;
                    _listener.ConnectionReceived += (sender, args) => OnClientConnected(sender, args);

                    await _listener.BindServiceNameAsync(_servicePort.ToString());

                    Debug.WriteLine("Listener started accepting request on port " + _listener.Information.LocalPort.ToString());
                }
                catch (Exception ex)
                {
                    // If this is an unknown status it means that the error is fatal and retry will likely fail.
                    if (SocketError.GetStatus(ex.HResult) == SocketErrorStatus.Unknown)
                    {
                        Debug.WriteLine("Start listening failed with fatal error: " + ex.Message);
                    }
                }
            }
        }

        /// <summary>
        /// Stops the service listener and message poller.
        /// </summary>
        public void StopService()
        {
            if (_listener != null)
            {
                try
                {
                    StopPolling();

                    _listener.Dispose();
                    _listener = null;

                    Debug.WriteLine("Listener stopped listening for requests");
                }
                catch (Exception ex)
                {
                    // If this is an unknown status it means that the error is fatal and retry will likely fail.
                    if (SocketError.GetStatus(ex.HResult) == SocketErrorStatus.Unknown)
                    {
                        Debug.WriteLine("Listener stopped listening for requests with fatal error: " + ex.Message);
                    }
                }
            }
        }

        /// <summary>
        /// A client has connected
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnClientConnected(StreamSocketListener sender, StreamSocketListenerConnectionReceivedEventArgs args)
        {
            if (sender == null) throw new ArgumentNullException("sender");
            if (args == null) throw new ArgumentNullException("args");

            Debug.WriteLine("Connection from {0}:{1} to {2}:{3} was established",
                args.Socket.Information.RemoteHostName.DisplayName,
                args.Socket.Information.RemotePort,
                args.Socket.Information.LocalAddress.DisplayName,
                args.Socket.Information.LocalPort);

            var clientKey = Guid.NewGuid();

            ActiveClients.Add(clientKey, args.Socket);

            ClientReader(args.Socket);

            args.Socket.Dispose();
            ActiveClients.Remove(clientKey);

            Debug.WriteLine("Connection from {0}:{1} to {2}:{3} was disconnected",
                args.Socket.Information.RemoteHostName.DisplayName,
                args.Socket.Information.RemotePort,
                args.Socket.Information.LocalAddress.DisplayName,
                args.Socket.Information.LocalPort);
        }

        /// <summary>
        /// A message was recived
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnMessageReceived(object sender, MessageReceivedEventArgs args)
        {
            foreach (var client in ActiveClients.ToList())
            {
                var results = SendMessage(client.Value, args.Message);

                if (results == false)
                {
                    Debug.WriteLine("Connection from {0}:{1} to {2}:{3} was disconnected",
                        client.Value.Information.RemoteHostName.DisplayName,
                        client.Value.Information.RemotePort,
                        client.Value.Information.LocalAddress.DisplayName,
                        client.Value.Information.LocalPort);

                    client.Value.Dispose();

                    ActiveClients.Remove(client.Key);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="socket"></param>
        /// <returns></returns>
        private bool ClientReader(StreamSocket socket)
        {
            try
            {
                using (DataReader reader = new DataReader(socket.InputStream))
                {
                    reader.InputStreamOptions = InputStreamOptions.Partial;

                    while (socket != null)
                    {
                        // Read message from client
                        uint messageLength = reader.LoadAsync(MessageReader.MaximumMessageSize).AsTask().GetAwaiter().GetResult();

                        if (reader.UnconsumedBufferLength > 0)
                        {
                            var readBuffer = new byte[reader.UnconsumedBufferLength];
                            reader.ReadBytes(readBuffer);

                            if (readBuffer.Length != messageLength)
                            {
                                // The underlying socket was closed before we were able to read the whole data. 
                                return false;
                            }

                            Debug.WriteLine("Write Buffer:");
                            Debug.Write(ArrayExtensions.HexDump(readBuffer));
                            WriteReceiver(readBuffer);
                            
                        }
                        Task.Delay(100);
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Send a message to all active clients
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="buffer"></param>
        /// <returns></returns>
        private bool SendMessage(StreamSocket socket, byte[] buffer)
        {
            if (buffer == null) throw new ArgumentNullException("buffer");
            if (socket == null || socket.OutputStream == null)
                return false;

            Debug.WriteLine("Send Buffer:");
            Debug.Write(ArrayExtensions.HexDump(buffer));

            try
            {
                using (DataWriter outputWriter = new DataWriter(socket.OutputStream))
                {
                    outputWriter.WriteBytes(buffer);
                    outputWriter.StoreAsync().AsTask().GetAwaiter().GetResult();
                    outputWriter.FlushAsync().AsTask().GetAwaiter().GetResult();

                    outputWriter.DetachStream();
                }
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }
    }
}

