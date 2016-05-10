using System;
using Android.Views;
using Android.OS;
using Android.Widget;
using Android.Support.V7.Widget;
using Android.Support.Design.Widget;
using System.Collections.Generic;

namespace BudgetTracker
{
	public class CategoriesFragment : Android.App.Fragment
	{
		private RecyclerView categoriesRecyclerView;
		private FloatingActionButton fab;

		private CategoryService categoryService;
		private CategoryTypeService categoryTypeService;
		private InputUtilities inputUtilities;
		private RecyclerView.Adapter categoriesAdapter;
		private IEnumerable<Category> categories;

		public CategoriesFragment (CategoryService categoryService, CategoryTypeService categoryTypeService, InputUtilities inputUtilities)
		{
			this.categoryService = categoryService;
			this.categoryTypeService = categoryTypeService;
			this.inputUtilities = inputUtilities;
		}

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			var view = inflater.Inflate (Resource.Layout.Categories, container, false);

			fab = view.FindViewById<FloatingActionButton> (Resource.Id.fab);
			fab.Click += AddCategory;

			categoriesRecyclerView = view.FindViewById<RecyclerView> (Resource.Id.categoriesRecyclerView);
			RecyclerView.LayoutManager categoriesLayoutManager = new LinearLayoutManager (this.Activity);
			categoriesRecyclerView.SetLayoutManager (categoriesLayoutManager);

			this.categoriesAdapter = new CategoriesAdapter (this.categories, this.categoryTypeService, this.inputUtilities);
			categoriesRecyclerView.SetAdapter (categoriesAdapter);

			this.categoryService.RetrieveCategories().ContinueWith ((categoryResult) => {
				this.categories = categoryResult.Result;
				this.UpdateAdapter ();
			});

			return view;
		}

		public void UpdateAdapter ()
		{
			if (this.categoriesAdapter != null) {
				this.categoriesAdapter.NotifyDataSetChanged ();
			}
		}

		public override void OnDestroyView ()
		{
			if (this.fab != null) {
				this.fab.Click -= AddCategory;
				this.fab.Dispose ();
				this.fab = null;
			}

			if (this.categoriesRecyclerView != null) {
				this.categoriesRecyclerView.Dispose ();
				this.categoriesRecyclerView = null;
			}

			base.OnDestroyView ();
		}

		public void AddCategory(object sender, EventArgs e) 
		{
			var toast = Toast.MakeText (this.Activity, "Clicked FAB", ToastLength.Short);
			toast.Show ();
		}
	}
}