using Bookstore_Razor.Data;
using Bookstore_Razor.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Bookstore_Razor.Pages.Categories
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _db;
		public List<Category> CategoyList {  get; set; }
		public IndexModel(ApplicationDbContext db)
		{
			_db = db;
		}
		public void OnGet()
        {
			CategoyList= _db.Categories.ToList();

		}
    }
}
