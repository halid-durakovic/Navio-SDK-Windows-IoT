using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Emlid.WindowsIot.Hardware.Components.Ublox.Ubx
{
    /// <summary>
    /// Index message types 
    /// </summary>
    public class PayloadIndexer
    {
        /// <summary>
        /// Find all message properties inherated from <see cref="MessageBase"/> with the <see cref="MessageAttribute"/> 
        /// and <see cref="PayloadIndexAttribute"/>.
        /// </summary>
        public static PayloadMetadata Find(Type type)
        {

            PayloadMetadata metadata = new PayloadMetadata();

            var message = type.GetTypeInfo().GetCustomAttribute<MessageAttribute>();
            var properties = IterateProps(type);

            metadata.IsConfiguration = type.GetTypeInfo().GetCustomAttributes(typeof(ConfigurationAttribute), true).Any();
            metadata.IsAcknowledged = type.GetTypeInfo().GetCustomAttributes(typeof(AcknowledgedAttribute), true).Any();
            metadata.ClassID = (byte)message.ClassID;
            metadata.MessageID = message.MessageID;
            metadata.MessageType = message.MessageType;
            metadata.MessageName = type.Name;
            metadata.SystemType = type;
            metadata.Payload = new List<PayloadProperties>();

            foreach (var property in properties)
            {
                metadata.Payload.Add(new PayloadProperties()
                {
                    PropertyName = property.Name,
                    PropertyType = property.PropertyType,
                    PropertyIndex = property.GetCustomAttribute<PayloadIndexAttribute>().Index,
                    PropertySize = property.GetCustomAttribute<PayloadIndexAttribute>().Size
                });
            }

            return metadata;
        }

        private static IEnumerable<PropertyInfo> IterateProps(Type baseType)
        {
            return IteratePropsInner(baseType, baseType.Name);
        }

        private static IEnumerable<PropertyInfo> IteratePropsInner(Type baseType, string baseName)
        {

            var props = from p in baseType.GetRuntimeProperties()
                        let attribute = p.GetCustomAttribute<PayloadIndexAttribute>()
                        where attribute != null
                        orderby attribute.Index
                        select p;

            foreach (var property in props)
            {
                var name = property.Name;
                var type = ListArgumentOrSelf(property.PropertyType);

                if (IsMarked(type))
                    foreach (var info in IteratePropsInner(type, name))
                        yield return info;
                else
                    yield return property;
            }
        }

        private static bool IsMarked(Type type)
        {
            return (type.GetTypeInfo().GetCustomAttributes(typeof(PayloadBlockAttribute), true).Any());
        }

        private static Type ListArgumentOrSelf(Type type)
        {
            if (!type.GetTypeInfo().IsGenericType)
                return type;
            if (type.GetGenericTypeDefinition() != typeof(IEnumerable<>))
                throw new Exception("Only IEnumerable<T> are allowed");
            return type.GetGenericArguments()[0];
        }
    }
}
