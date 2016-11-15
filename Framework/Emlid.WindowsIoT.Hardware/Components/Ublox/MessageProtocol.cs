namespace Emlid.WindowsIot.Hardware.Components.Ublox
{
    /// <summary>
    /// Message Protocol Type
    /// </summary>
    public enum MessageProtocol : byte
    {
        /// <summary>
        /// GNSS receiver messages are based on NMEA 0183 Version 4.0 protocol.
        /// </summary>
        NEMA = 0x00,

        /// <summary>
        /// GNSS receiver messages are based on a u-blox proprietary protocol.
        /// </summary>
        UBX = 0x01
    }
}
