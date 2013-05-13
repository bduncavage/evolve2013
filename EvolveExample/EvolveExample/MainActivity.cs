using System;

using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;

using EvolveExample.Fragments;

using SupportFragmentActivity = Android.Support.V4.App.FragmentActivity;
using SupportFragmentManager = Android.Support.V4.App.FragmentManager;
using EvolveExample.Services;
using Android.Content;

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

        #region lifecycle

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

            // start our master service
            // We could alternatively start this from our Application class
            var intent = new Intent(this, typeof(MasterService));
            StartService(intent);
        }

        protected override void OnResume()
        {
            base.OnResume ();
            UpdateServiceForeground(true);
        }

        protected override void OnPause()
        {
            base.OnPause ();
            UpdateServiceForeground(false);
        }

        protected override void OnDestroy ()
        {
            base.OnDestroy ();
            nav.NavigationItemActivated -= nav_NavigationItemSelected;
        }

        #endregion

        #region private methods

        private void UpdateServiceForeground(bool isForeground)
        {
            if (MasterService.Instance != null) {
                MasterService.Instance.UpdateForegroundState(isForeground);
            }
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
            var transitionType = FragmentTransit.None;
            if (didChange) {
                if (is_dual_pane) {
                    transitionType = FragmentTransit.FragmentFade;
                    containerResId = Resource.Id.content_fragment_container;
                } else {
                    transitionType = FragmentTransit.FragmentOpen;
                    addToBackStack = true;
                    containerResId = Resource.Id.fragment_container;
                }

                var transaction = SupportFragmentManager.BeginTransaction();
                transaction.SetTransition((int)transitionType);

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

        #endregion
    }
}


