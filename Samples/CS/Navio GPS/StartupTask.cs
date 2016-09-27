using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Emlid.WindowsIot.Hardware;
using Emlid.WindowsIot.Hardware.Boards.Navio;

using Windows.ApplicationModel.Background;

namespace Emlid.WindowsIot.Samples.NavioPosition
{
    /// <summary>
    /// Start-up task.
    /// </summary>
    public sealed class StartupTask : IBackgroundTask
    {
        /// <summary>
        /// Executes the task.
        /// </summary>
        public void Run(IBackgroundTaskInstance taskInstance)
        {
            // Initialize position device and reset to default config.
            var positioning = NavioPositionDevice.Initialize();

            // Verify the position sensor is connected, accessable and ready for use.
            if (positioning.IsConnected == false)
                Debug.WriteLine("****** GPS Device is not connected *******");
        }
    }
}
