using System;

namespace Emlid.WindowsIot.Hardware.Components.Ublox.Ubx
{
    /// <summary>
    /// Indicate the UBX message structure
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    sealed class MessageAttribute : Attribute
    {
        /// <summary>
        /// Class ID of the UBX message
        /// </summary>
        public MessageClass ClassID { get; private set; }

        /// <summary>
        /// Message ID of the UBX message
        /// </summary>
        public byte MessageID { get; private set; }

        /// <summary>
        /// Message type of the UBX message
        /// </summary>
        public MessageType MessageType { get; private set; }

        /// <summary>
        /// Creates a message attribute instance
        /// </summary>
        public MessageAttribute(MessageClass classId, byte messageId, MessageType messageType)
        {
            ClassID = classId;
            MessageID = messageId;
            MessageType = messageType;
        }
    }

    /// <summary>
    /// Indicate a playload field location index
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    sealed class PayloadIndexAttribute : Attribute
    {
        /// <summary>
        /// Payload field location index
        /// </summary>
        public int Index { get; private set; }

        /// <summary>
        /// Payload field size in bytes
        /// </summary>
        public int Size { get; private set; }

        /// <summary>
        /// Creates a payload attribute instance
        /// </summary>
        public PayloadIndexAttribute(int index)
        {
            Index = index;
        }

        /// <summary>
        /// Creates a payload attribute instance
        /// </summary>
        public PayloadIndexAttribute(int index , int size)
        {
            Index = index;
            Size = size;
        }
    }

    /// <summary>
    /// Indicate the UBX message is a configuration message 
    /// </summary>
    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    sealed class ConfigurationAttribute : Attribute
    {

    }

    /// <summary>
    /// Indicate the UBX message provides an acknowledgment response message 
    /// </summary>
    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    sealed class AcknowledgedAttribute : Attribute
    {

    }

    /// <summary>
    /// Indicate the UBX message is a payload repeat block structure 
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    sealed class PayloadBlockAttribute : Attribute
    {

    }
}
