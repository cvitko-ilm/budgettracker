using System;
using System.Linq;
using System.Collections.Generic;
using SharedPCL;
using System.Threading.Tasks;
using SQLite;

namespace BudgetTracker
{
	public class CategoryService : ICategoryService
	{
		private SQLiteAsyncConnection _dbConn { get; set; }
		private object locker { get; set;}

		public CategoryService (string dbPath)
		{
			_dbConn = new SQLiteAsyncConnection (dbPath);
			locker = new object ();
		}

		public async Task<IList<Category>> RetrieveCategories() {
			// don't need this as the sqlite-net pcl async ORM does the locking for you, but as an example.
			//lock (locker) {
				return await _dbConn.Table<Category> ().ToListAsync ();
			//}
		}

		public async Task<Category> RetrieveCategoryByName(string name) {
			var categories = await this.RetrieveCategories ();

			return categories.Where (x => x.Name == name).FirstOrDefault ();
		}

		public async Task<Category> RetrieveCategoryById(string id)
		{
			var categories = await this.RetrieveCategories ();

			return categories.FirstOrDefault (x => x.Id == id);
		}

		public async Task<bool> Delete(Category category)
		{

			var categories = await this.RetrieveCategories ();

			var deletedCategory = categories.Where (x => x.Id == category.Id).FirstOrDefault ();
			if (deletedCategory == null) {
				return true;
			}

			var rowsDeleted = await _dbConn.DeleteAsync (category);
			return rowsDeleted > 0;
		}

		public async Task<bool> Insert(Category category) {
			if (category.Id == null) {
				category.Id = Guid.NewGuid ().ToString();
			}
			var rowsAffected = await _dbConn.InsertAsync (category);

			return rowsAffected > 0;
		}

		private IList<Category> CloneList(IList<Category> categoryList)
		{
			IList<Category> clonedList = new List<Category> ();
			if (categoryList == null) {
				return null;
			}

			foreach (var category in categoryList) {
				Category newCategory = new Category ();
				newCategory.CategoryType = category.CategoryType;
				newCategory.Description = category.Description;
				newCategory.Id = category.Id;
				newCategory.Name = category.Name;
				clonedList.Add (newCategory);
			}

			return clonedList;
		}

		public Task InitializeService()
		{
			List<Task> tasks = new List<Task> ();

			if (!this.TableExists ()) {
				tasks.AddRange (new List<Task> {
				_dbConn.CreateTableAsync<Category> (),
				   this.Insert (new Category () {
						Name = "Food",
						Description = "Eating out",
						CategoryType = CategoryType.Expense,
						Id = Guid.NewGuid ().ToString()
					}),
				   this.Insert (new Category () {
						Name = "Utilities",
						Description = "Cable and Internet",
						CategoryType = CategoryType.Expense,
						Id = Guid.NewGuid ().ToString ()
					}),
				   this.Insert (new Category () {
						Name = "Vacation",
						Description = "Travel budget",
						CategoryType = CategoryType.Expense,
						Id = Guid.NewGuid ().ToString ()
					}),
				   this.Insert (new Category () {
						Name = "Paycheck",
						Description = "Salary",
						CategoryType = CategoryType.Income,
						Id = Guid.NewGuid ().ToString()
					}),
					this.Insert (new Category () {
						Name = "Donations",
						Description = "Charitable Giving",
						CategoryType = CategoryType.Expense,
						Id = Guid.NewGuid ().ToString()
					})
				});
			}

			return Task.WhenAll (tasks);
		}

		private bool TableExists()
		{
			string cmdText = "SELECT name FROM sqlite_master WHERE type='table' AND name=?";
			var cmd = _dbConn.ExecuteScalarAsync<string>(cmdText, typeof (Category).Name );
			return cmd.Result != null;
		}
	}
}

