using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Emlid.WindowsIot.Hardware.Components.Ublox.Ubx;

namespace Emlid.WindowsIot.Hardware.Components.Ublox
{
    /// <summary>
    /// 
    /// </summary>
    public interface IMessageResult
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
