using System;

namespace Emlid.WindowsIot.Hardware.Components.Ublox.Ubx
{
    /// <summary>
    /// Defines the UBX protocol port configuration.
    /// </summary>
    /// 
    [Config]
    [CLSCompliant(false)]
    [Message(MessageClass.Cfg, 0x00, MessageType.Get | MessageType.Set)]
    public class PortConfiguration : MessageBase
    {
        /// <summary>
        /// Creates an instance with the specified values
        /// </summary>
        public PortConfiguration()
        {
            PortId = 0x04; // default to SPI
            TxReady = new BitField16();
            Mode = new BitField32();
            Reserved2 = new byte[4];
            InProtoMask = new BitField16();
            OutProtoMask = new BitField16();
            Flags = new BitField16();
            Reserved3 = new byte[2];
        }

        /// <summary>
        /// Port identifier number
        /// </summary>
        [PayloadIndex(0)]
        public byte PortId { get; set; }

        /// <summary>
        /// Reserved
        /// </summary>
        [PayloadIndex(1)]
        public byte Reserved1 { get; set; }

        /// <summary>
        /// TX ready PIN configuration
        /// </summary>
        [PayloadIndex(2)]
        public BitField16 TxReady { get; set; }

        /// <summary>
        /// SPI mode flags
        /// </summary>
        [PayloadIndex(3)]
        public BitField32 Mode { get; set; }

        /// <summary>
        /// Reserved
        /// </summary>
        [PayloadIndex(4, 4)]
        public byte[] Reserved2 { get; set; }

        /// <summary>
        /// Mask describing which input protocols are active.
        /// </summary>
        [PayloadIndex(5)]
        public BitField16 InProtoMask { get; set; }

        /// <summary>
        /// Mask describing which output protocols are active.
        /// </summary>
        [PayloadIndex(6)]
        public BitField16 OutProtoMask { get; set; }

        /// <summary>
        /// Flags bit mask
        /// </summary>
        [PayloadIndex(7)]
        public BitField16 Flags { get; set; }

        /// <summary>
        /// Reserved
        /// </summary>
        [PayloadIndex(8, 2)]
        public byte[] Reserved3 { get; set; }

        /// <summary>
        /// Is UBX protocol in
        /// </summary>
        public bool IsInUbx
        {
            get { return InProtoMask[0]; }
            set { InProtoMask[0] = value; }
        }

        /// <summary>
        /// Is NMEA protocol in
        /// </summary>
        public bool IsInNmea
        {
            get { return InProtoMask[1]; }
            set { InProtoMask[1] = value; }
        }

        /// <summary>
        /// Is RTCM protocol in
        /// </summary>
        public bool IsInRtcm
        {
            get { return InProtoMask[2]; }
            set { InProtoMask[2] = value; }
        }

        /// <summary>
        /// Is RTCM3 protocol in
        /// </summary>
        public bool IsInRtcm3
        {
            get { return InProtoMask[5]; }
            set { InProtoMask[5] = value; }
        }

        /// <summary>
        /// Is UBX protocol out
        /// </summary>
        public bool IsOutUbx
        {
            get { return OutProtoMask[0]; }
            set { OutProtoMask[0] = value; }
        }

        /// <summary>
        /// Is NMEA protocol out
        /// </summary>
        public bool IsOutNmea
        {
            get { return OutProtoMask[1]; }
            set { OutProtoMask[1] = value; }
        }

        /// <summary>
        /// Is RTCM3 protocol out
        /// </summary>
        public bool IsOutRtcm3
        {
            get { return OutProtoMask[5]; }
            set { OutProtoMask[5] = value; }
        }
    }
}
