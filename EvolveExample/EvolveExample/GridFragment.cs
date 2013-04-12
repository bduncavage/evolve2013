
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
	public class GridFragment : SupportFragment
	{
		private GridView grid_view;
		private GridAdapter adapter;

		private List<string> data_list = new List<string>() { "Foo", "bar", "baz" };

		public GridFragment() : base()
		{
		}

		public override void OnCreate (Bundle savedInstanceState)
		{
			// fill up the list with a bunch of junk
			while (data_list.Count () < 100) {
				data_list.AddRange(data_list);
			}
			base.OnCreate (savedInstanceState);
			adapter = new GridAdapter(this, Activity, Resource.Layout.grid_item, data_list);
		}

		public override View OnCreateView (LayoutInflater inflater, ViewGroup parent, Bundle data)
		{
			var view = inflater.Inflate(Resource.Layout.grid, null, false);
			grid_view = view.FindViewById<GridView>(Resource.Id.grid_view);
			grid_view.Adapter = adapter;
			return view;
		}

		#region Adapter

		private class GridAdapter : ArrayAdapter
		{
			private GridFragment fragment;

			public override int Count {
				get {
					return fragment.data_list.Count();
				}
			}
			public GridAdapter(GridFragment fragment, Context context, int textViewResId, List<string> objects) : base(context, textViewResId, objects)
			{
				this.fragment = fragment;
			}

			public override Java.Lang.Object GetItem (int position)
			{
				if (position == fragment.data_list.Count() - 1) {
					fragment.data_list.AddRange(fragment.data_list);
					NotifyDataSetChanged();
				}
				return fragment.data_list[position];
			}
			public override View GetView (int position, View convertView, ViewGroup parent)
			{
				return base.GetView (position, convertView, parent);
			}
		}

		#endregion
	}
}

