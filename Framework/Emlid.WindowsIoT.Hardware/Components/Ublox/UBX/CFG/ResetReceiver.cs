using System;

namespace Emlid.WindowsIot.Hardware.Components.Ublox.Ubx
{
    /// <summary>
    /// Defines the reset receiver / clear backup data UBX protocol message.
    /// </summary>
    [CLSCompliant(false)]
    [Config]
    [Message(MessageClass.Cfg, 0x04, MessageType.Command)]
    public class ResetReceiver : MessageBase
    {
        /// <summary>
        /// Creates an instance with the specified values
        /// </summary>
        public ResetReceiver()
        {
            BbrMask = new BitField16();
            ResetMode = ResetMode.SoftwareReset;
        }

        /// <summary>
        /// BBR Sections to clear.
        /// </summary>
        [PayloadIndex(0)]
        public BitField16 BbrMask { get; set; } 

        /// <summary>
        /// Reset mode
        /// </summary>
        [PayloadIndex(1)]
        public ResetMode ResetMode { get; set; }

        /// <summary>
        /// Reserved
        /// </summary>
        [PayloadIndex(2)]
        public byte Reserved1 { get; }

        /// <summary>
        /// Is almanac
        /// </summary>
        public bool IsAlm
        {
            get { return BbrMask[1]; }
            private set { BbrMask[1] = value; }
        }

        /// <summary>
        /// Is health
        /// </summary>
        public bool IsHealth
        {
            get { return BbrMask[2]; }
            private set { BbrMask[2] = value; }
        }

        /// <summary>
        /// Is klobuchar parameters
        /// </summary>
        public bool IsKlob
        {
            get { return BbrMask[3]; }
            private set { BbrMask[3] = value; }
        }

        /// <summary>
        /// Is position
        /// </summary>
        public bool IsPos
        {
            get { return BbrMask[4]; }
            private set { BbrMask[4] = value; }
        }

        /// <summary>
        /// Is clock drift
        /// </summary>
        public bool IsClkd
        {
            get { return BbrMask[5]; }
            private set { BbrMask[5] = value; }
        }

        /// <summary>
        /// Is oscillator Parameter
        /// </summary>
        public bool IsOsc
        {
            get { return BbrMask[6]; }
            private set { BbrMask[6] = value; }
        }

        /// <summary>
        /// Is UTC correction plus GPS leap seconds parameters
        /// </summary>
        public bool IsUtc
        {
            get { return BbrMask[7]; }
            private set { BbrMask[7] = value; }
        }

        /// <summary>
        /// Is RTC
        /// </summary>
        public bool IsRtc
        {
            get { return BbrMask[8]; }
            private set { BbrMask[8] = value; }
        }

        /// <summary>
        /// Is ephemeris
        /// </summary>
        public bool IsEph
        {
            get { return BbrMask[15]; }
            private set { BbrMask[15] = value; }
        }
    }

    /// <summary>
    /// Reset mode 
    /// </summary>
    public enum ResetMode : byte
    {
        /// <summary>
        /// Hardware reset (Watchdog) immediately 
        /// </summary>
        HardwareRest = 0x00,

        /// <summary>
        /// Controlled Software reset
        /// </summary>
        SoftwareReset = 0x01,

        /// <summary>
        /// Controlled Software reset (GNSS only)
        /// </summary>
        GnssOnly = 0x02,

        /// <summary>
        /// Hardware reset (Watchdog) after shutdown
        /// </summary>
        ShutdownReset = 0x04,

        /// <summary>
        /// Controlled GNSS stop
        /// </summary>
        GnssStop = 0x08,

        /// <summary>
        /// Controlled GNSS start
        /// </summary>
        GnssStart = 0x09
    }
}
