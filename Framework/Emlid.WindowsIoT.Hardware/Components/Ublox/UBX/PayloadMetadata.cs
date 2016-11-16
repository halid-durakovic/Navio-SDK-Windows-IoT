using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emlid.WindowsIot.Hardware.Components.Ublox.Ubx
{
    /// <summary>
    /// Defines the payload metadata of the UBX message
    /// </summary>
    public class PayloadMetadata 
    {
        /// <summary>
        /// Class ID of the UBX message
        /// </summary>
        public byte ClassID { get; set; }

        /// <summary>
        /// Message ID of the UBX message
        /// </summary>
        public byte MessageID { get; set; }

        /// <summary>
        /// Message type of the UBX message
        /// </summary>
        public MessageType MessageType { get; set; }

        /// <summary>
        /// Indicate if the message is a UBX configuration message 
        /// </summary>
        public bool IsConfiguration { get; set; }

        /// <summary>
        /// Indicate the UBX message is acknowledged by a response message 
        /// </summary>
        public bool IsAcknowledged { get; set; }

        /// <summary>
        /// Message name of the UBX message
        /// </summary>
        public string MessageName { set; get; }

        /// <summary>
        /// Message type of the UBX message
        /// </summary>
        public Type SystemType { set; get; }

        /// <summary>
        /// Message payload properties of the UBX message
        /// </summary>
        public List<PayloadProperties> Payload { set; get; }
    }
}
