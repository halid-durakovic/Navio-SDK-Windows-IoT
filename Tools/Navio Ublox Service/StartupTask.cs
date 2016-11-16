using System;
using System.Diagnostics;
using Windows.Foundation;
using Windows.System.Threading;
using Windows.ApplicationModel.Background;

namespace Emlid.WindowsIot.NavioUbloxService
{
    /// <summary>
    /// Start-up task.
    /// </summary>
    public sealed class StartupTask : IBackgroundTask
    {
        private BackgroundTaskDeferral backgroundTaskDeferral;
        private NavioUcenterDevice ubloxService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="taskInstance"></param>
        public void Run(IBackgroundTaskInstance taskInstance)
        {
            // Get the deferral and save it to local variable so that the app stays alive
            backgroundTaskDeferral = taskInstance.GetDeferral();
            taskInstance.Canceled += OnCanceled;

            try
            {
                // Initialize ublox device and reset to default config.
                ubloxService = NavioUcenterDevice.Initialize();

                IAsyncAction asyncAction = ThreadPool.RunAsync((workItem) =>
                {
                    ubloxService.StartService(1031);
                });

                Debug.WriteLine("Ublox background task is running...");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exception starting task: {0}", ex.Message);
            }
        }

        private void OnCanceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            // Stop the ublox service
            ubloxService.StopService();

            // Release the deferral so that the app can be stopped.
            backgroundTaskDeferral.Complete();
        }
    }
}
