using System;
using System.Collections.Generic;

using Android.App;
using Android.Content;
using Android.OS;

namespace EvolveExample.Services
{
	public interface IPluginService
	{
		void HandleStartCommand(Intent intent, int startId);
		void HandleCommand(Intent intent);
		void OnDestroy();
		void OnForegroundStateChanged(bool isForeground);
	}

	[Service]
	[IntentFilter (new[] { MASTER_SERVICE_INTENT_FILTER })]
	public class MasterService : Service
	{
		public const string MASTER_SERVICE_INTENT_FILTER = "com.myapp.services.MasterService";
        private readonly long PING_INTERVAL = 7000;

		private HashSet<IPluginService> plugins;
		private LocalBinder local_binder;

		public override void OnCreate()
		{
			base.OnCreate();

			local_binder = new LocalBinder(this);

            plugins = new HashSet<IPluginService> {
                new DevicePingPluginService(this, PING_INTERVAL),
            };
		}

		public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
		{
            HashSet<IPluginService> localPlugins = null;
            lock (plugins) {
                localPlugins = plugins;
            }
			foreach (var plugin in localPlugins) {
				plugin.HandleStartCommand(intent, startId);
			}

			return StartCommandResult.NotSticky;
		}

		public override IBinder OnBind(Intent intent)
		{
			// This service is meant to run in the same process as our activity
			// so we can return a reference to it.
			return (IBinder)local_binder;
		}

		public override void OnDestroy()
		{
			base.OnDestroy();
			HashSet<IPluginService> localPlugins = null;
			lock (plugins) {
				localPlugins = plugins;
			}
			foreach (var plugin in localPlugins) {
				plugin.OnDestroy();
			}
		}

		public void RegisterPlugin(IPluginService plugin)
		{
			lock (plugins) {
				plugins.Add(plugin);
			}
		}

		public void UnregisterPlugin(IPluginService plugin)
		{
			lock (plugins) {
				plugins.Remove(plugin);
			}
		}

		private class LocalBinder : Binder
		{
			private MasterService master_service;

			public LocalBinder(MasterService masterService)
			{
				master_service = masterService;
			}

			public  MasterService Service {
				get { return master_service; }
			}
		}
	}
}

