using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emlid.WindowsIot.Hardware.Components.Ublox.Ubx
{
    /// <summary>
    /// Defines the payload properties of the UBX message
    /// </summary>
    public class PayloadProperties
    {
        /// <summary>
        /// Payload property index of the UBX message
        /// </summary>
        public int PropertyIndex { set; get; }

        /// <summary>
        /// Payload property name of the UBX message
        /// </summary>
        public string PropertyName { set; get; }

        /// <summary>
        /// Payload property type of the UBX message
        /// </summary>
        public Type PropertyType { set; get; }

        /// <summary>
        /// Payload property size of the UBX message
        /// </summary>
        public int PropertySize { set; get; }
    }
}
