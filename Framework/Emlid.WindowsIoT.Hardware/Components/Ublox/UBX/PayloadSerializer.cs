using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Emlid.WindowsIot.Hardware.Components.Ublox.Ubx
{
    /// <summary>
    /// 
    /// </summary>
    public class PayloadSerializer
    {
        /// <summary>
        /// 
        /// </summary>
        public PayloadSerializer(byte[] message)
        {
            MemoryStream = new MemoryStream(message);
            BinaryReader = new BinaryReader(MemoryStream);
        }

        /// <summary>
        /// 
        /// </summary>
        public PayloadSerializer()
        {
            MemoryStream = new MemoryStream();
            BinaryWriter = new BinaryWriter(MemoryStream);
        }

        /// <summary>
        /// 
        /// </summary>
        public MemoryStream MemoryStream { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public BinaryReader BinaryReader { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public BinaryWriter BinaryWriter { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="propertyType"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public object ReadValue(Type propertyType, int size)
        {
            object propertyValue = null;

            if (propertyType.IsArray && size > 0)
            {
                propertyValue = Read(propertyType, size);
            }
            else if (propertyType.GetTypeInfo().IsPrimitive)
            {
                propertyValue = Read(propertyType);
            }
            else if (typeof(BitField).IsAssignableFrom(propertyType))
            {
                propertyValue = Read(propertyType);
            }
            else if (propertyType.GetTypeInfo().IsEnum)
            {
                object numericValue = Read(Enum.GetUnderlyingType(propertyType));
                propertyValue = Enum.ToObject(propertyType, numericValue);
            }

            return propertyValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="propertyType"></param>
        /// <param name="value"></param>
        public void WriteValue(Type propertyType, object value)
        {          
            if (propertyType.GetTypeInfo().IsPrimitive)
            {
                Write(propertyType, value);
            }
            else if (propertyType.GetTypeInfo().IsArray)
            {
                Write(propertyType, value);
            }
            else if (typeof(BitField).IsAssignableFrom(propertyType))
            {
                Write(propertyType, value);
            }
            else if (propertyType.GetTypeInfo().IsEnum)
            {
                Write(Enum.GetUnderlyingType(propertyType), value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="value"></param>
        private void Write(Type type, object value)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));

            BinaryWriter writer = BinaryWriter;

            if (type == typeof(byte))  //U1
            {
                writer.Write((byte)value);
            }
            else if (type == typeof(byte[])) //CH
            {
                writer.Write((byte[])value);
            }
            else if (type == typeof(short)) //I2
            {
                writer.Write((short)value);
            }
            else if (type == typeof(ushort)) //U2
            {
                writer.Write((ushort)value);
            }
            else if (type == typeof(long)) //I4
            {
                writer.Write((long)value);
            }
            else if (type == typeof(ulong)) //U4
            {
                writer.Write((ulong)value);
            }
            else if (type == typeof(int))  //I4
            {
                writer.Write((int)value);
            }
            else if (type == typeof(uint)) //U4
            {
                writer.Write((uint)value);
            }
            else if (type == typeof(float)) //R4
            {
                writer.Write((float)value);
            }
            else if (type == typeof(double)) //R8
            {
                writer.Write((double)value);
            }
            else if (typeof(BitField).IsAssignableFrom(type))  // X1 X2 X4
            {
                writer.Write(((BitField)value).GetBytes());
            }
            else
            {
                throw new NotSupportedException(nameof(type));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        private object Read(Type type, int size = 0)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));

            BinaryReader reader = BinaryReader;

            if(type == typeof(byte)) //U1
            {
                return reader.ReadByte();
            }
            else if (type == typeof(byte[])) //CH
            {
                return reader.ReadBytes(size);
            }
            else if (type == typeof(short)) //I2
            {
                return reader.ReadInt16();
            }
            else if (type == typeof(ushort)) //U2
            {
                return reader.ReadUInt16();
            }
            else if (type == typeof(long) || type == typeof(int)) //I4
            {
                return reader.ReadInt32();
            }
            else if (type == typeof(ulong) || type == typeof(uint)) //U4
            {
                return reader.ReadUInt32();
            }
            else if (type == typeof(float)) //R4
            {
                return reader.ReadSingle();
            }
            else if (type == typeof(double)) //R8
            {
                return reader.ReadDouble();
            }
            else if (type == typeof(BitField8)) //X1
            {
                return BitField8.FromBytes(reader.ReadBytes(1));
            }
            else if (type == typeof(BitField16)) //X2
            {
                return BitField16.FromBytes(reader.ReadBytes(2));
            }
            else if (type == typeof(BitField32)) //X4
            {
                return BitField32.FromBytes(reader.ReadBytes(4));
            }
            else
            {
                throw new NotSupportedException(nameof(type));
            }
        }
    }
}