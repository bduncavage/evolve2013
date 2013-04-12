using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

using SupportFragmentActivity = Android.Support.V4.App.FragmentActivity;
using SupportFragmentManager = Android.Support.V4.App.FragmentManager;

namespace EvolveExample
{
	[Activity (Label = "EvolveExample", MainLauncher = true)]
	public class Activity1 : SupportFragmentActivity
	{
		private NavFragment nav;
		private GridFragment grid_fragment;
		private ImageFragment image_fragment;

		bool is_dual_pane;
		private int current_nav_position = -1;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.main_layout);

			is_dual_pane = Resources.GetBoolean(Resource.Boolean.has_two_panes);

			var fm = SupportFragmentManager;
			var transaction = fm.BeginTransaction();
			nav = new NavFragment();

			if (is_dual_pane) {
				grid_fragment = new GridFragment();
				current_nav_position = 0;
				transaction.Replace(Resource.Id.nav_fragment_container, nav);
				transaction.Replace(Resource.Id.content_fragment_container, grid_fragment);
			} else {
				transaction.Replace(Resource.Id.fragment_container, nav);
			}

			transaction.Commit();

			nav.NavigationItemActivated += nav_NavigationItemSelected;
		}

		protected override void OnDestroy ()
		{
			base.OnDestroy ();
			nav.NavigationItemActivated -= nav_NavigationItemSelected;
		}

		private void nav_NavigationItemSelected(object sender, NavigationEventArgs args)
		{
			var didChange = false;
			var addToBackStack = false;

			if (current_nav_position != args.NavItemPosition) {
				current_nav_position = args.NavItemPosition;
				didChange = true;
			}

			var containerResId = 0;

			if (didChange) {
				if (is_dual_pane) {
					containerResId = Resource.Id.content_fragment_container;
				} else {
					addToBackStack = true;
					containerResId = Resource.Id.fragment_container;
				}

				var transaction = SupportFragmentManager.BeginTransaction();

				if (current_nav_position == 0) {
					grid_fragment = grid_fragment ?? new GridFragment();
					transaction.Replace(containerResId, grid_fragment);
				} else if (current_nav_position == 1) {
					image_fragment = new ImageFragment(Resource.Drawable.android_flavors);
					transaction.Replace(containerResId, image_fragment);
				} else if (current_nav_position == 2) {
					image_fragment = new ImageFragment(Resource.Drawable.xamarin);
					transaction.Replace(containerResId, image_fragment);
				}

				if (addToBackStack) {
					transaction.AddToBackStack(null);
				}

				transaction.Commit();
			}
		}
	}
}


