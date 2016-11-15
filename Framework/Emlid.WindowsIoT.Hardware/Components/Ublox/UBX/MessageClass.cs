using System;

namespace Emlid.WindowsIot.Hardware.Components.Ublox.Ubx
{
        /// <summary>
        /// Message class IDs
        /// </summary>
        [Flags]
        public enum MessageClass
        {
        /// <summary>
        /// Navigation Results Messages: Position, Speed, Time, Acceleration, Heading, DOP, SVs used
        /// </summary>
        Nav = 0x1,

        /// <summary>
        /// Receiver Manager Messages: Satellite Status, RTC Status
        /// </summary>
        Rxm = 0x2,

        /// <summary>
        /// Information Messages: Printf-Style Messages, with IDs such as Error, Warning, Notice
        /// </summary>
        Inf = 0x4,

        /// <summary>
        /// Ack/Nak Messages: Acknowledge or Reject messages to CFG input messages
        /// </summary>
        Ack = 0x5,

        /// <summary>
        /// Configuration Input Messages: Set Dynamic Model, Set DOP Mask, Set Baud Rate, etc.
        /// </summary>
        Cfg = 0x06,

        /// <summary>
        /// Firmware Update Messages: Memory/Flash erase/write, Reboot, Flash identification, etc.
        /// </summary>
        Upd = 0x09,

        /// <summary>
        /// Monitoring Messages: Communication Status, CPU Load, Stack Usage, Task Status
        /// </summary>
        Mon = 0x0A,

        /// <summary>
        /// AssistNow Aiding Messages: Ephemeris, Almanac, other A-GPS data input
        /// </summary>
        Iad = 0x0B,

        /// <summary>
        /// Timing Messages: Time Pulse Output, Time Mark Results
        /// </summary>
        Tim = 0x0D,
        
        /// <summary>
        /// AssistNow Aiding Messages: Ephemeris, Almanac, other A-GPS data input
        /// </summary>
        Esf = 0x10,
        
        /// <summary>
        /// External Sensor Fusion Messages: External Sensor Measurements and Status Information
        /// </summary>
        Mga = 0x13,
        
        /// <summary>
        /// Logging Messages: Log creation, deletion, info and retrieval
        /// </summary>
        Log = 0x21,
        
        /// <summary>
        /// Security Feature Messages
        /// </summary>
        Sec = 0x27,

        /// <summary>
        /// High Rate Navigation Results Messages: High rate time, position, speed, heading
        /// </summary>
        Hnr = 0x28,
    }
}
