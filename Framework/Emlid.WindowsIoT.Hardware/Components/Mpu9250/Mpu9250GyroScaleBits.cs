using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emlid.WindowsIot.Hardware.Components.Mpu9250
{
    /// <summary>
    /// Possible gyro scales and their register bit settings.
    /// </summary>
    [Flags]
    public enum Mpu9250GyroScaleBits : byte
    {
        GFSDPS250 = 0x00,
        GFSDPS500 = 0x08,
        GFSDPS1000 = 0x10,
        GFSDPS2000 = 0x18
    }
}
