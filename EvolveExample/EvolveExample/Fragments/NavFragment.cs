
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

using SupportListFragment = Android.Support.V4.App.ListFragment;

namespace EvolveExample.Fragments
{
	public class NavigationEventArgs : EventArgs
	{
		public int NavItemPosition;

	}

	public interface INavigationView
	{
		event EventHandler<NavigationEventArgs> NavigationItemActivated;
	}

	public class NavFragment : SupportListFragment, INavigationView
	{
		List<String> nav_options = new List<string>() { "Option One", "Option Two", "Option Three" };

		public event EventHandler<NavigationEventArgs> NavigationItemActivated;

		public NavFragment() : base()
		{
		}

		public override void OnViewStateRestored (Bundle savedInstanceState)
		{
			base.OnViewStateRestored (savedInstanceState);
			ListAdapter = new ArrayAdapter(Activity, Resource.Layout.nav_item, nav_options);
		}

		public override void OnResume ()
		{
			base.OnResume ();
			ListView.ItemClick += listview_ItemClick;
		}

		public override void OnPause ()
		{
			base.OnPause ();
			ListView.ItemClick -= listview_ItemClick;
		}

		private void listview_ItemClick(object sender, AdapterView.ItemClickEventArgs args)
		{
			var h = NavigationItemActivated;
			if (h != null) {
				h(this, new NavigationEventArgs { NavItemPosition = args.Position });
			}
		}

	}
}

