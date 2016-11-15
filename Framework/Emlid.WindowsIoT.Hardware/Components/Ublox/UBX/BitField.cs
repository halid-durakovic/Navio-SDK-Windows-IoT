using System;

namespace Emlid.WindowsIot.Hardware.Components.Ublox.Ubx
{
    /// <summary>
    /// Represents a bit field 1 byte in length.
    /// </summary>
    [CLSCompliant(false)]
    public class BitField8 : BitField
    {
        /// <summary>
        /// 
        /// </summary>
        public BitField8() : base(8) 
        {
            UnsetAll();
        }

        /// <summary>
        /// Creates a new BitField from the specified array of bytes.
        /// </summary>
        /// <param name="data">The array of bytes to create the BitField from.</param>
        /// <returns></returns>
        public new static BitField8 FromBytes(byte[] data)
        {
            var bf = new BitField8();
            Array.Copy(data, bf._field, data.Length);
            return bf;
        }
    }

    /// <summary>
    /// Represents a bit field 2 byte in length.
    /// </summary>
    [CLSCompliant(false)]
    public class BitField16 : BitField
    {
        /// <summary>
        /// 
        /// </summary>
        public BitField16() : base(16)
        {
            UnsetAll();
        }

        /// <summary>
        /// Creates a new BitField from the specified array of bytes.
        /// </summary>
        /// <param name="data">The array of bytes to create the BitField from.</param>
        /// <returns></returns>
        public new static BitField16 FromBytes(byte[] data)
        {
            var bf = new BitField16();
            Array.Copy(data, bf._field, data.Length);
            return bf;
        }
    }

    /// <summary>
    /// Represents a bit field 4 bytes in length.
    /// </summary>
    [CLSCompliant(false)]
    public class BitField32 : BitField
    {
        /// <summary>
        /// 
        /// </summary>
        public BitField32() : base(32)
        {
            UnsetAll();
        }

        /// <summary>
        /// Creates a new BitField from the specified array of bytes.
        /// </summary>
        /// <param name="data">The array of bytes to create the BitField from.</param>
        /// <returns></returns>
        public new static BitField32 FromBytes(byte[] data)
        {
            var bf = new BitField32();
            Array.Copy(data, bf._field, data.Length);
            return bf;
        }
    }

    /// <summary>
    /// Represents a bit field of arbitrary length.
    /// </summary>
    [CLSCompliant(false)]
    public class BitField
    {
        internal byte[] _field;

        /// <summary>
        /// Creates a new instance of the <see cref="BitField"/> class with the specified number of bits.
        /// </summary>
        /// <param name="bits">The number of bits in the bit field. This value must be a multiple of 8.</param>
        public BitField(int bits)
        {
            if (bits % 8 != 0)
            {
                throw new ArgumentException("Bit count must be a multiple of 8");
            }

            _field = new byte[bits / 8];
        }

        internal BitField(byte[] data)
        {
            _field = data;
        }

        /// <summary>
        /// Accesses the bit at the specified index in the bit field.
        /// </summary>
        /// <param name="i">The index of the bit to access.</param>
        /// <returns></returns>
        public bool this[int i]
        {
            get 
            {
                return ((_field[i / 8] >> (i % 8)) & 1) == 1;
            }
            set
            {
                    _field[i / 8] = (byte)((_field[i / 8] & ~(1 << (i % 8))) | ((value ? 1 << i % 8 : 0)));
            }
        }

        /// <summary>
        /// The number of bits in the BitField.
        /// </summary>
        public int Bits
        {
            get { return _field.Length * 8; }
        }

        /// <summary>
        /// The number of bytes in the BitField.
        /// </summary>
        public int Bytes
        {
            get { return _field.Length; }
        }

        /// <summary>
        /// Unsets all the flags in the bitfield.
        /// </summary>
        public void UnsetAll()
        {
            Array.Clear(_field, 0, _field.Length);
        }

        /// <summary>
        /// Sets all the flags in the bitfield.
        /// </summary>
        public void SetAll()
        {
            for(int i = 0; i < _field.Length; i++)
            {
                _field[i] = 0xFF;
            }
        }

        /// <summary>
        /// Returns the number of set bits.
        /// </summary>
        /// <returns>The number of set bits.</returns>
        public int GetSetCount()
        {
            int n = 0;
            foreach(byte b in _field)
            {
                n += NumberOfSetBits(b);
            }
            return n;
        }

        /// <summary>
        /// Returns the number of unset bits.
        /// </summary>
        /// <returns>The number of unset bits.</returns>
        public int GetUnsetCount()
        {
            return Bits - GetSetCount();
        }

        /// <summary>
        /// Inverts the flags in the bit field.
        /// </summary>
        public void Invert()
        {
            for(int i = 0; i < _field.Length; i++)
            {
                _field[i] = (byte)~_field[i];
            }
        }

        /// <summary>
        /// Creates a new BitField from the specified array of bytes.
        /// </summary>
        /// <param name="data">The array of bytes to create the BitField from.</param>
        /// <returns></returns>
        public static BitField FromBytes(byte[] data)
        {
            var bf = new BitField(data.Length * 8);
            Array.Copy(data, bf._field, data.Length);
            return bf;
        }

        /// <summary>
        /// Returns the BitField as an array of bytes.
        /// </summary>
        /// <returns></returns>
        public byte[] GetBytes()
        {
            return _field;
        }

        /// <summary>
        /// Returns the number of set bits
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public static int NumberOfSetBits(uint i)
        {
            i = i - ((i >> 1) & 0x55555555);
            i = (i & 0x33333333) + ((i >> 2) & 0x33333333);
            return (int)(((i + (i >> 4)) & 0x0F0F0F0F) * 0x01010101) >> 24;
        }
    }
}
