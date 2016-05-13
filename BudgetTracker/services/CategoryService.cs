using System;
using System.Linq;
using System.Collections.Generic;
using SharedPCL;
using System.Threading.Tasks;
using System.Net.Http;
using ModernHttpClient;
using Newtonsoft.Json;

namespace BudgetTracker
{
	public class CategoryService : ICategoryService
	{
		private const string azureCategoryAPI = "http://cvitko-budgettrackerapi.azurewebsites.net/tables/Category";

		private HttpClient httpClient { get; }

		public CategoryService ()
		{
			this.httpClient = new HttpClient (new NativeMessageHandler ());
		}

		public async Task<IList<Category>> RetrieveCategories() {

			var response = await httpClient.GetAsync (azureCategoryAPI);

			response.EnsureSuccessStatusCode ();

			string jsonStr = await response.Content.ReadAsStringAsync ();

			return JsonConvert.DeserializeObject<IList<Category>> (jsonStr);
		}

		public async Task<Category> RetrieveCategoryByName(string name) {
			var categories = await this.RetrieveCategories ();
			return categories.FirstOrDefault (x => x.Name == name);
		}

		public async Task<Category> RetrieveCategoryById(string id)
		{
			var response = await httpClient.GetAsync (azureCategoryAPI + "/" + id);

			response.EnsureSuccessStatusCode ();

			string jsonStr = await response.Content.ReadAsStringAsync ();

			return JsonConvert.DeserializeObject<Category> (jsonStr);
		}

		public async Task<bool> Delete(Category category)
		{
			var categories = await this.RetrieveCategories ();
			var deletedCategory = categories.Where (x => x.Id == category.Id).FirstOrDefault ();
			if (deletedCategory == null) {
				return true;
			}

			var response = await httpClient.DeleteAsync(azureCategoryAPI + "/" + deletedCategory.Id);

			response.EnsureSuccessStatusCode ();

			string jsonStr = await response.Content.ReadAsStringAsync ();

			return JsonConvert.DeserializeObject<bool> (jsonStr);
		}

		public async Task<bool> Insert(Category category) {
			if (category.Id == null) {
				category.Id = Guid.NewGuid ().ToString();
			}

			StringContent content = new StringContent (JsonConvert.SerializeObject (category));

			var response = await httpClient.PostAsync(azureCategoryAPI, content);

			response.EnsureSuccessStatusCode ();

			string jsonStr = await response.Content.ReadAsStringAsync ();

			return JsonConvert.DeserializeObject<bool> (jsonStr);
		}

		public Task InitializeService()
		{
			return Task.Run(() => { });
		}
	}
}

