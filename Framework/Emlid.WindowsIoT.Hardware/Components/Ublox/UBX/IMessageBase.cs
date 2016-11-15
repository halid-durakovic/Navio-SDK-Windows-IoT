namespace Emlid.WindowsIot.Hardware.Components.Ublox.Ubx
{
    /// <summary>
    /// 
    /// </summary>
    public interface IMessageBase
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        bool TryParse(byte[] message);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        byte[] ToArray();
    }
}