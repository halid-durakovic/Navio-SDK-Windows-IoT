using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Emlid.WindowsIot.Hardware;
using Emlid.WindowsIot.Hardware.Boards.Navio;
using Emlid.WindowsIot.Hardware.Components.Mpu9250;


using Windows.ApplicationModel.Background;

namespace Emlid.WindowsIot.Samples.NavioMotion
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
            // Initialize motion device and reset to default config.
            var motion = NavioMotionDevice.Initialize(Mpu9250Placement.P1, true);
            motion.FusionEnabled = true;

            // Verify the motion sensor is connected, accessable and ready for use.
            if (motion.IsConnected == false)
                Debug.WriteLine("Motion Device is not connected");

            int i = 0;
            while (i < 1000)
            {
                motion.Update();
                Debug.WriteLine(motion.SensorReading.ToString());
                Task.Delay(100).Wait();
                i = i + 1;
            }
        }
    }

}
