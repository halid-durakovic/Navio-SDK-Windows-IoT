using System;

namespace Emlid.WindowsIot.Hardware.Components.Ublox
{
    /// <summary>
    /// Event arguments for MessageReceived event.
    /// </summary>
    public class MessageReceivedEventArgs : EventArgs
    {
        /// <summary>
        /// The received message bytes.
        /// </summary>
        /// 
        public byte[] Message { get; private set; }

        /// <summary>
        /// The received message type.
        /// </summary>
        public Type MessageType { get { return MessageResult?.GetType(); } }

        /// <summary>
        /// The received message object results.
        /// </summary>
        [CLSCompliant(false)]
        public IMessageResult MessageResult { get; private set; }

        /// <summary>
        /// The received message protocol.
        /// </summary>
        public MessageProtocol MessageProtocol { get; private set; }

        /// <summary>
        /// Instantiate MessageReceivedEventArgs.
        /// </summary>
        /// <param name="message">The received message bytes.</param>
        /// <param name="result">The received message object.</param>
        /// <param name="type">The received message protocol type.</param>
        internal MessageReceivedEventArgs(byte[] message, IMessageResult result, MessageProtocol type)
        {
            Message = message;
            MessageResult = result;
            MessageProtocol = type;
        }
    }
}
