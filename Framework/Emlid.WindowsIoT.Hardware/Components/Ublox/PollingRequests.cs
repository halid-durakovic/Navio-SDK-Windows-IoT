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
    public class PollingRequests : List<Type>
    {
    
        /// <summary>
        /// 
        /// </summary>
        public PollingRequests()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="capacity"></param>
        public PollingRequests(int capacity) : base(capacity)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="collection"></param>
        public PollingRequests(IEnumerable<Type> collection) : base(collection)
        {
        }
    }
}
