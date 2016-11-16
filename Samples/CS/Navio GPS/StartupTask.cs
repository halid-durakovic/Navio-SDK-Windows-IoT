using System;
using System.Diagnostics;
using Windows.ApplicationModel.Background;

using Emlid.WindowsIot.Hardware.Boards.Navio;
using Emlid.WindowsIot.Hardware.Components.Ublox;

namespace Emlid.WindowsIot.Samples.NavioPosition
{
    /// <summary>
    /// Start-up task.
    /// </summary>
    public sealed class StartupTask : IBackgroundTask
    {
        private BackgroundTaskDeferral backgroundTaskDeferral;
        private NavioPositionDevice positioning;

        /// <summary>
        /// Executes the task.
        /// </summary>
        public void Run(IBackgroundTaskInstance taskInstance)
        {
            // Get the deferral and save it to local variable so that the app stays alive
            backgroundTaskDeferral = taskInstance.GetDeferral();
            taskInstance.Canceled += OnCanceled;

            // Initialize position device and reset to default config.
            positioning = NavioPositionDevice.Initialize();

            // Enable reading changed handler 
            positioning.GeodeticSensorChanged += Positioning_ReadingChanged;
       
            // Verify the position sensor is connected, accessable and ready for use.
            if (positioning.IsConnected == false)
            Debug.WriteLine("****** GPS Device is not connected *******");

        }

        private void Positioning_ReadingChanged(object sender, GeodeticSensorReading args)
        {
            Debug.WriteLine("Latitude: " + args.Latitude);
            Debug.WriteLine("Longitude: " + args.Longitude);
            Debug.WriteLine("Altitude: " + args.HeightAboveSeaLevel);
        }

        private void OnCanceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            // Stop the ublox service
            positioning.StopPolling();

            // Release the deferral so that the app can be stopped.
            backgroundTaskDeferral.Complete();
        }    
    }
}
