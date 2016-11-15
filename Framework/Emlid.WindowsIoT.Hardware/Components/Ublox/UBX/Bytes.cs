using System;
using System.Linq;

namespace Emlid.WindowsIot.Hardware.Components.Ublox.Ubx
{
    /// <summary>
    /// Represents a bytes of arbitrary length.
    /// </summary>
    public class Bytes
    {
        private readonly byte[] _bytes;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="values"></param>
        public Bytes(params byte[] values)
        {
            if (values == null || values.Length < 1)
            {
                throw new ArgumentOutOfRangeException();
            }
            _bytes = values;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="size"></param>
        public Bytes(int size)
        {
            _bytes = new byte[size];
        }

        /// <summary>
        /// 
        /// </summary>
        public int Length
        {
            get
            {
                return _bytes.Length;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            return obj is Bytes && this._bytes.SequenceEqual((obj as Bytes)._bytes);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return (_bytes.Length > 2 ? _bytes[_bytes.Length - 3] : _bytes[0]) * _bytes.Length;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public static implicit operator Bytes(byte[] value)
        {
            return new Bytes(value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public static explicit operator byte[] (Bytes value)
        {
            return value.ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static Bytes FromString(string str)
        {
            byte[] b = new byte[str.Length];

            for (int i = 0; i < b.Length; i++)
            {
                b[i] = (byte)str[i];
            }

            return b;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public byte[] ToArray()
        {
            return (byte[])_bytes.Clone();
        }
    }
}
