using Emlid.WindowsIot.Hardware.Components.Ublox.Ubx;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Emlid.WindowsIot.Hardware.Components.Ublox
{
    /// <summary>
    /// 
    /// </summary>
    public class MessageFactory
    {
        private Dictionary<int, Type> _messages = new Dictionary<int, Type>();

        /// <summary>
        /// 
        /// </summary>
        public MessageFactory()
        {
            Assembly assembly = typeof(MessageBase).GetTypeInfo().Assembly;

            var messageTypes = from type in assembly.GetTypes()
                               let attribute = type.GetTypeInfo().GetCustomAttribute(typeof(MessageAttribute)) as MessageAttribute
                               where attribute != null
                               select new { ClassID = attribute.ClassID, MessageID = attribute.MessageID, ObjectType = type };

            _messages = messageTypes.ToDictionary(k => new { ClassID = (byte)k.ClassID, MessageID = k.MessageID }.GetHashCode(), v => v.ObjectType);
        }
        
        /// <summary>
        /// 
        /// </summary>
        public IEnumerable Messages
        {
            get { return _messages; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual bool TryMapping(int hashcode, out Type mapping)
        {
            mapping = null;
            if (_messages.TryGetValue(hashcode, out mapping))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual IMessageResult Invoke(byte[] message)
        {
            if (message.Length < Neom8nDevice.MininumMessageSize || message.Length > Neom8nDevice.MaximumMessageSize)
                return null;

            int hashcode = new { ClassID = message[2], MessageID = message[3] }.GetHashCode();

            return Invoke(hashcode);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual IMessageResult Invoke(int hashcode)
        {
            Type type;
            if (TryMapping(hashcode, out type))
            {
                return (IMessageResult)Activator.CreateInstance(type);
            }
            else
            {
                return null;
            }
        }
    }
}
