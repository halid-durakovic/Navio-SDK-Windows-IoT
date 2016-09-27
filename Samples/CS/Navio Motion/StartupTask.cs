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
            var motion = NavioMotionDevice.Initialize();

            // Verify the motion sensor is connected, accessable and ready for use.
            if (motion.IsConnected == false)
                Debug.WriteLine("****** Motion Device is not connected *******");

            while (true)
            {
                //string format = "ax:{0:0.000} ay:{1:0.000} az:{2:0.000} gx:{3:0.000} gy:{4:0.000} gz:{5:0.000} mx:{6:0.000} my:{7:0.000} mz:{8:0.000}";
                //double[] data = motion.ReadMotion9();
                //Debug.WriteLine(string.Format(format, data[0], data[1], data[2], data[3], data[4], data[5], data[6], data[7], data[8]));

                Debug.WriteLine(motion.ReadAll().ToString());

                //Debug.WriteLine("");
                Task.Delay(1000).Wait();
            }
        }
    }

}
