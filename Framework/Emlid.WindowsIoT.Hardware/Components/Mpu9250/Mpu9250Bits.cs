using System;

namespace Emlid.WindowsIot.Hardware.Components.Mpu9250
{
    /// <summary>
    /// Bitmask for the <see cref="Mpu9250Register"/> register.
    /// </summary>
    [Flags]
    public enum Mpu9250Bits : byte
    {
        /// <summary>
        /// No bits set.
        /// </summary>
        None = 0x00,

        /// <summary>
        /// </summary>
        ReadFlag = 0x80,

        /// <summary>
        /// </summary>
        Sleep = 0x40,

        /// <summary>
        /// </summary>
        Reset = 0x80,


        //#define BITS_CLKSEL 0x07
        //#define MPU_CLK_SEL_PLLGYROX 0x01
        //#define MPU_CLK_SEL_PLLGYROZ 0x03
        //#define MPU_EXT_SYNC_GYROX 0x02


        //#define BITS_FS_MASK                0x18
        //#define BITS_DLPF_CFG_256HZ_NOLPF2  0x00
        //#define BITS_DLPF_CFG_188HZ         0x01
        //#define BITS_DLPF_CFG_98HZ          0x02
        //#define BITS_DLPF_CFG_42HZ          0x03
        //#define BITS_DLPF_CFG_20HZ          0x04
        //#define BITS_DLPF_CFG_10HZ          0x05
        //#define BITS_DLPF_CFG_5HZ           0x06
        //#define BITS_DLPF_CFG_2100HZ_NOLPF  0x07
        //#define BITS_DLPF_CFG_MASK          0x07
        //#define BIT_INT_ANYRD_2CLEAR        0x10
        //#define BIT_RAW_RDY_EN              0x01
        //#define BIT_I2C_IF_DIS              0x10

    }
}
