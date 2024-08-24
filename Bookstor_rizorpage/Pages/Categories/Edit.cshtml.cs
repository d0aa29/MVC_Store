using Bookstore_Razor.Data;
using Bookstore_Razor.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Bookstore_Razor.Pages.Categories
{
	[BindProperties]
	public class EditModel : PageModel
    {
		//TempData["Succes"] = "Category Edited successfully";
		public readonly ApplicationDbContext _db;
	
		public Category Category { get; set; }
		public EditModel(ApplicationDbContext db)
		{
			_db = db;
		}
		public void OnGet(int? id)
		{
			if (id != null && id != 0)
			{
				Category = _db.Categories.Find(id);
			}
		}
		public IActionResult OnPost()
		{
			if (ModelState.IsValid)
			{
				_db.Categories.Update(Category);
				_db.SaveChanges();
				TempData["Succes"] = "Category Edited successfully";
				return RedirectToPage("Index");
			}
			return Page();
		}
	}
}
