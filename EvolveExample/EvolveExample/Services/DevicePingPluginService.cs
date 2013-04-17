using System;

using Android.Content;
using Android.App;
using Java.Net;
using System.Net;
using Android.OS;
using System.Threading;
using System.Collections.Specialized;
using System.Text;
using Android.Widget;

namespace EvolveExample.Services
{
    public class DevicePingPluginService : IPluginService
    {
        private const string TAG = "DevicePingPluginService";

        public static readonly string PING_SERVICE_EXTRA = "devicePingService";

        private readonly object monitor = new object();
        private bool is_pinging = false;

        private long ping_interval;
        private MasterService master_service;
        PendingIntent repeating_pending_intent;

        public DevicePingPluginService (MasterService masterService, long pingInterval)
        {
            ping_interval = pingInterval;
            master_service = masterService;

            StartPinging();
        }

        #region IPluginService implementation

        public void HandleStartCommand(Intent intent, int startId)
        {
            HandleCommand(intent);
        }

        public void HandleCommand(Intent intent)
        {
            if (intent.HasExtra(PING_SERVICE_EXTRA)) {
                PerformPing();
            }
        }

        private void PerformPing()
        {
            lock (monitor) {
                if (is_pinging) {
                    return;
                }
                is_pinging = true;
            }

            ThreadPool.QueueUserWorkItem( o => {
                using (var wb = new WebClient())
                {
                    var data = new NameValueCollection();
                    data["model"] = Build.Model;
                    data["product"] = Build.Product;
                    data["android_sdk_int"] = Build.VERSION.SdkInt.ToString();

                    try {
                        // obviously, this does nothing, but illustrates the point.
                        var response = wb.UploadValues("http://xamarin.com", "POST", data);
                        Android.Util.Log.Info(TAG, "Finished ping, response was: {0}", Encoding.ASCII.GetString(response));
                        // show a toast
                        // make sure we do this on the main thread.
                        var handler = new Handler(Looper.MainLooper);
                        handler.Post(() => {
                            Toast.MakeText(this.master_service, "Ping Complete!", ToastLength.Short).Show();
                        });
                    } catch (Exception e) {
                        // TODO: do some real error handling.
                        Android.Util.Log.Error(TAG, "Error when pinging: {0}", e.ToString());
                    }
                    lock(monitor) {
                        is_pinging = false;
                    }
                }
            });
        }

        private void StartPinging()
        {
            var alarmManager = (AlarmManager)master_service.GetSystemService(Context.AlarmService);
            var pingIntent = new Intent(MasterService.MASTER_SERVICE_INTENT_FILTER);
            pingIntent.PutExtra(PING_SERVICE_EXTRA, true);
            repeating_pending_intent = PendingIntent.GetService(master_service, -1, pingIntent, PendingIntentFlags.UpdateCurrent);
            alarmManager.SetInexactRepeating(AlarmType.Rtc, DateTime.UtcNow.Millisecond, ping_interval, repeating_pending_intent);
        }

        private void StopPinging()
        {
            var alarmManager = (AlarmManager)master_service.GetSystemService(Context.AlarmService);
            alarmManager.Cancel(repeating_pending_intent);
            repeating_pending_intent.Cancel();

            repeating_pending_intent = null;
        }

        public void OnDestroy()
        {
            StopPinging();
            master_service = null;
        }

        public void OnForegroundStateChanged(bool isForeground)
        {
            if (isForeground) {
                StartPinging();
            } else {
                StopPinging();
            }
        }

        #endregion
    }
}

