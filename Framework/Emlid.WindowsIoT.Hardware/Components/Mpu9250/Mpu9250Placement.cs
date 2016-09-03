
namespace Emlid.WindowsIot.Hardware.Components.Mpu9250
{
    /// <summary>
    /// Defines the specific placement location of the device on the board allowing proper remapping of the x/y/z axis.
    /// </summary>
    public enum Mpu9250Placement : int
    {
        /// <summary>
        /// Axis remap placement locations 0.
        /// </summary>
        P0 = 0,

        /// <summary>
        /// Axis remap placement locations 1 (default).
        /// </summary>
        P1 = 1,

        /// <summary>
        /// Axis remap placement locations 2.
        /// </summary>
        P2 = 2,

        /// <summary>
        /// Axis remap placement locations 3.
        /// </summary>
        P3 = 3,

        /// <summary>
        /// Axis remap placement locations 4.
        /// </summary>
        P4 = 4,

        /// <summary>
        /// Axis remap placement locations 5.
        /// </summary>
        P5 = 5,

        /// <summary>
        /// Axis remap placement locations 6.
        /// </summary>
        P6 = 6,

        /// <summary>
        /// Axis remap placement locations 7.
        /// </summary>
        P7 = 7
    }
}
