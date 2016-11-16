using System;

using Emlid.WindowsIot.Common;
using System.Diagnostics;
using System.Reflection;
using System.IO;
using System.Linq;

namespace Emlid.WindowsIot.Hardware.Components.Ublox.Ubx
{
    /// <summary>
    /// Message base
    /// </summary>
    [CLSCompliant(false)]
    public abstract class MessageBase : IMessageBase, IMessageResult
    {
        #region Lifetime

        /// <summary>
        /// Creates a message instance
        /// </summary>
        public MessageBase()
        {
            // set message attributes
            PayloadMetadata = PayloadIndexer.Find(this.GetType());
        }

        #endregion

        #region Private Fields

        #endregion

        #region Public Properties

        /// <summary>
        /// Header (sync char 1 and 2) of the UBX message 
        /// </summary>
        public static byte[] MessageHeader { get; private set; } = new byte[2] { 0xB5, 0x62 };

        /// <summary>
        /// Class ID of the UBX message
        /// </summary>
        public byte ClassID
        {
            get { return PayloadMetadata.ClassID; }
        }

        /// <summary>
        /// Message ID of the UBX message
        /// </summary>
        public byte MessageID
        {
            get { return PayloadMetadata.MessageID; }
        }

        /// <summary>
        /// Message type of the UBX message
        /// </summary>
        public MessageType MessageType
        {
            get { return PayloadMetadata.MessageType; }
        }

        /// <summary>
        /// Indicate if the message is a UBX configuration message 
        /// </summary>
        /// <returns></returns>
        public bool IsConfig
        {
            get { return PayloadMetadata.IsConfiguration; }
        }

        /// <summary>
        /// Indicate the UBX message is acknowledged by a response message
        /// </summary>
        /// <returns></returns>
        public bool IsAcknowledged
        {
            get { return PayloadMetadata.IsAcknowledged; }
        }

        /// <summary>
        /// Lenght of the payload excluding sync chars, class and message id's, lenght and checksum fields
        /// of the UBX message
        /// </summary>
        public short PayloadLength
        {
            get { return (short)Payload.Length; }
        }

        /// <summary>
        /// Payload of the UBX message
        /// </summary>
        public byte[] Payload
        {
            get
            {
                var serializer = new PayloadSerializer();
                foreach (var property in PayloadMetadata.Payload)
                {
                    var value = GetObjectProperty(property.PropertyName, this);
                    serializer.WriteValue(property.PropertyType, value);
                }
                return serializer.MemoryStream.ToArray();
            }

        }

        /// <summary>
        /// Payload metadata of the UBX message
        /// </summary>
        public PayloadMetadata PayloadMetadata { get; private set; }

        /// <summary>
        /// Checksum of the UBX message 
        /// </summary>
        public ushort Checksum
        {
            get
            {
                return GetChecksum(ClassID, MessageID, PayloadLength, Payload);
            }

            private set { Checksum = value; }
        }

        /// <summary>
        /// Returns a string representation of the message
        /// </summary>
        public override string ToString()
        {
            return String.Format("ClassID: 0x{1:X2} MessageID: {0x{2:X2}}", ClassID, MessageID);
        }

        /// <summary>
        /// Returns a number suitable for use in hashing algorithms and data structures 
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return new { ClassID, MessageID }.GetHashCode();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public bool TryParse(byte[] message)
        {
            // Validate
            if (message.Length < Neom8nDevice.MininumMessageSize)
                return false;

            if (!ArrayExtensions.AreEqual(MessageHeader, 0, message, 0, 2))
                return false;

            if (!ArrayExtensions.AreEqual(new byte[] { ClassID }, 0, message, 2, 1))
                return false;

            if (!ArrayExtensions.AreEqual(new byte[] { MessageID }, 0, message, 3, 1))
                return false;

            try
            {
                // Check payload length
                if (PayloadLength != (short)(message[5] << 8 | message[4]))
                    return false;

                // Set properties
                var payload = new byte[message.Length - 8];
                Array.ConstrainedCopy(message, 6, payload, 0, payload.Length);

                var serializer = new PayloadSerializer(payload);
                foreach (var property in PayloadMetadata.Payload)
                {
                    var value = serializer.ReadValue(property.PropertyType , property.PropertySize);
                    SetObjectProperty(property.PropertyName, value, this);
                }

                // Set checksum
                if (Checksum != (ushort)((message[message.Length - 2]) | (message[message.Length - 1] << 8)))
                    return false;
            }
            catch
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public byte[] ToArray()
        {
            byte[] results = new byte[8];

            if (IsConfig)
            {               
                results = new byte[PayloadLength + 8];
            }

            // Write header
            Array.ConstrainedCopy(MessageHeader, 0, results, 0, MessageHeader.Length);

            // Write class id and message id proprities
            results[2] = ClassID;
            results[3] = MessageID;

            if (IsConfig)
            {
                // Write payload length
                results[4] = (byte)(PayloadLength);
                results[5] = (byte)(PayloadLength >> 8);

                // Write payload
                Array.ConstrainedCopy(Payload, 0, results, 6, Payload.Length);
            }
            else
            {
                // Write 0 payload length
                results[4] = 0x00;
                results[5] = 0x00;
            }

            // Write checksum
            var checksum = GetChecksum(results, 2, results.Length - 2);
            results[results.Length - 2] = (byte)(checksum);
            results[results.Length - 1] = (byte)(checksum >> 8);

            return results;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Checksum algorithm
        /// </summary>
        /// <param name="classid"></param>
        /// <param name="messageid"></param>
        /// <param name="legnth"></param>
        /// <param name="payload"></param>
        /// <returns>Returns calculated checksum</returns>
        private ushort GetChecksum(byte classid, byte messageid, short legnth, byte[] payload)
        {
            var results = new byte[payload.Length + 4];
            results[0] = classid;
            results[1] = messageid;
            results[2] = (byte)(legnth);
            results[3] = (byte)(legnth >> 8);
            if (payload != null)
                Array.ConstrainedCopy(payload, 0, results, 4, payload.Length);

            return GetChecksum(results);
        }


        /// <summary>
        /// Checksum algorithm
        /// </summary>
        /// <param name="payload">Message payload</param>
        /// <returns>Returns calculated checksum</returns>
        private static ushort GetChecksum(byte[] payload)
        {
            return GetChecksum(payload, 0, payload.Length);
        }

        /// <summary>
        /// Checksum algorithm
        /// </summary>
        /// <param name="payload">Message payload</param>
        /// <param name="indexStart">Location to start reading bytes</param>
        /// <param name="length">Number of bytes to read</param>
        /// <returns>Returns calculated checksum</returns>
        private static ushort GetChecksum(byte[] payload, int indexStart, int length)
        {
            unchecked
            {
                uint crcA = 0;
                uint crcB = 0;
                if (payload.Length > 0)
                {
                    for (int i = indexStart; i < length; i++)
                    {
                        crcA += payload[i];
                        crcB += crcA;
                    }
                    crcA &= 0xff;
                    crcB &= 0xff;
                }
                return (ushort)(crcA | (crcB << 8));
            }
        }

        private void SetObjectProperty(string propertyName, object value, object obj)
        {
            PropertyInfo propertyInfo = GetType().GetProperty(propertyName);

            // make sure object has the property we are after
            if (propertyInfo != null && propertyInfo.CanWrite)
            {
                if (value != null)
                    propertyInfo.SetValue(obj, value, null);
            }
        }

        private object GetObjectProperty(string propertyName, object obj)
        {
            PropertyInfo propertyInfo = GetType().GetProperty(propertyName);

            // make sure object has the property we are after
            if (propertyInfo != null && propertyInfo.CanRead)
            {
                return propertyInfo.GetValue(obj);
            }

            return null;
        }

        #endregion
    }
}
