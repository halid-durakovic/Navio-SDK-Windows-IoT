using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Emlid.WindowsIot.Hardware.Components.Mpu9250
{
    /// <summary>
    /// Possible accelerometer scales and their register bit settings.
    /// </summary>
    [Flags]
    public enum Mpu9250AccelScaleBits : byte
    {
        AFS2G = 0x00,
        AFS4G = 0x08,
        AFS8G = 0x10,
        AFS16G = 0x18
    }
}
