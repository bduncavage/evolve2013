
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

using SupportFragment = Android.Support.V4.App.Fragment;

namespace EvolveExample
{
	public class ImageFragment : SupportFragment
	{
		private int drawable_res_id = 0;

		public ImageFragment() : base()
		{
		}

		public ImageFragment(int drawableResId) : base()
		{
			drawable_res_id = drawableResId;
		}

		public override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			if (savedInstanceState != null) {
				drawable_res_id = savedInstanceState.GetInt("drawable_res_id", 0);
			}
		}

		public override View OnCreateView (LayoutInflater inflater, ViewGroup parent, Bundle data)
		{
			var view = inflater.Inflate(Resource.Layout.image, parent, false);
			if (drawable_res_id > 0) {
				view.FindViewById<ImageView>(Resource.Id.image_view).SetImageDrawable(Resources.GetDrawable(drawable_res_id));
			}
			return view;
		}

		public override void OnSaveInstanceState (Bundle savedInstanceState)
		{
			base.OnSaveInstanceState (savedInstanceState);
			savedInstanceState.PutInt("drawable_res_id", drawable_res_id);
		}
	}
}

