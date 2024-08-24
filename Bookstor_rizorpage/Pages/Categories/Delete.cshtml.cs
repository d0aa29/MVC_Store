using Bookstore_Razor.Data;
using Bookstore_Razor.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Bookstore_Razor.Pages.Categories
{
    public class DeleteModel : PageModel
    {
        //TempData["Succes"] = "Category Deleteed successfully";

        public readonly ApplicationDbContext _db;
		[BindProperty]
		public Category Category { get; set; }
		public DeleteModel(ApplicationDbContext db)
		{
			_db = db;
		}
		public void OnGet(int? id)
		{
			if (id != null && id != 0)
			{
				Category = _db.Categories.FirstOrDefault(c => c.Id == id);
			}
		}

		public IActionResult OnPost()
		{
			Category obj = _db.Categories.Find(Category.Id);
			if (obj == null)
			{
				return NotFound();
			}
			_db.Categories.Remove(obj);
			_db.SaveChanges();
			TempData["Succes"] = "Category Deleteed successfully";
			return RedirectToPage("Index");
			
		}
	}
}
