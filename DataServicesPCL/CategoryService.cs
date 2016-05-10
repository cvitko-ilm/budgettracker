using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using SQLite;

namespace BudgetTracker
{
	public class CategoryService
	{
		private readonly SQLiteAsyncConnection _dbConn = null;

		private object locker = new object ();

		public CategoryService (SQLiteAsyncConnection dbConn)
		{
			_dbConn = dbConn;
			this.CreateTableData ().Wait();
		}

		public Task CreateTableData ()
		{
			List<Task> tasks = new List<Task> {
				_dbConn.CreateTableAsync<Category> (),
			   this.Insert (new Category () {
					Name = "Food",
					Description = "Eating out",
					CategoryType = CategoryType.Expense,
					Id = Guid.NewGuid ()
				}),
		       this.Insert (new Category () {
					Name = "Utilities",
					Description = "Cable and Internet",
					CategoryType = CategoryType.Expense,
					Id = Guid.NewGuid ()
				}),
		       this.Insert (new Category () {
					Name = "Vacation",
					Description = "Travel budget",
					CategoryType = CategoryType.Expense,
					Id = Guid.NewGuid ()
				}),
		       this.Insert (new Category () {
					Name = "Paycheck",
					Description = "Salary",
					CategoryType = CategoryType.Income,
					Id = Guid.NewGuid ()
				}),
				this.Insert (new Category () {
					Name = "Donations",
					Description = "Charitable Giving",
					CategoryType = CategoryType.Expense,
					Id = Guid.NewGuid ()
				})
				       
			};

			return Task.WhenAll (tasks);
		}

		public Task<List<Category>> RetrieveCategories ()
		{
			// don't need this as the sqlite-net pcl ORM does the locking for you, but as an example.
			lock (locker) {
				return _dbConn.Table<Category> ().ToListAsync ();
			}
		}

		public async Task<Category> RetrieveCategoryByName (string name)
		{
			var categories = await this.RetrieveCategories ();

			return categories.Where (x => x.Name == name).FirstOrDefault ();
		}

		public Task Delete (Category category)
		{
			return _dbConn.DeleteAsync(category);
		}

		public Task Insert (Category category)
		{
			if (category.Id == Guid.Empty) {
				category.Id = Guid.NewGuid ();
			}

			return _dbConn.InsertAsync(category);
		}

		//private async Task<bool> TableExists()
		//{
		//	string cmdText = "SELECT name FROM sqlite_master WHERE type='table' AND name=?";
		//	var result = await this._dbConn.ExecuteScalarAsync<Category>(cmdText, new object[] { "Category"});
		//	return result != null;
		//}
	}
}

