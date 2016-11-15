using System;
using System.Diagnostics;
using Windows.ApplicationModel.Background;

using Emlid.WindowsIot.Hardware.Boards.Navio;
using Emlid.WindowsIot.Hardware.Components.Ublox;
using Emlid.WindowsIot.Hardware.Components.Ublox.Ubx;
using Emlid.WindowsIot.Common;
using System.Text;

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

            // Enable message received handler
            positioning.MessageReceived += Positioning_MessageReceived;

            // Enable reading changed handler 
            positioning.GeodeticSensorChanged += Positioning_ReadingChanged;

            // Reset
            //positioning.Reset();

            ////  Turn on polling for messages
            positioning.SetPollingRate(0x01, 0x02, 0x01);
            //positioning.SetPollingRate(0x01, 0x03, 0x00);
            //positioning.SetPollingRate(0x01, 0x04, 0x00);
            //positioning.SetPollingRate(0x01, 0x30, 0x00);
        
            // Verify the position sensor is connected, accessable and ready for use.
            if (positioning.IsConnected == false)
            Debug.WriteLine("****** GPS Device is not connected *******");

            Debug.WriteLine(positioning.SoftwareVersion);

        }

        private void Positioning_MessageReceived(object sender, MessageReceivedEventArgs args)
        {
            //if (args.MessageType == typeof(Acknowledge))
            //{

            //}
            //if (args.MessageType == typeof(NotAcknowledge))
            //{

            //}
          
            ////if (args.MessageType == typeof(NavigationStatus))
            ////{
            ////    NavigationStatus message = (NavigationStatus)args.MessageResult;
            ////    Debug.WriteLine("Fix Status: " + message.Fix.ToString());
            ////}
            ////else if (args.MessageType == typeof(DilutionPrecision))
            ////{
            ////    DilutionPrecision message = (DilutionPrecision)args.MessageResult;
            ////    Debug.WriteLine("Time Dilution: " + message.Time.ToString());
            ////}
            //else
            //{
            //    //Debug.WriteLine(ArrayExtensions.HexDump(args.Message));
            //}
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
            positioning.Stop();

            // Release the deferral so that the app can be stopped.
            backgroundTaskDeferral.Complete();
        }    
    }
}
