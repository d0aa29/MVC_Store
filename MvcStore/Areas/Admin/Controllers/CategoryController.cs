
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Store.DataAccess.Data;
using Store.DataAccess.Repository.IRepository;
using Store.Models;
using Store.Utility;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;

namespace Mystore.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize ( Roles = SD.Role_Admin)]
	public class CategoryController : Controller
	{
		private readonly IUnitOfWork _UnitOfWork;
		public CategoryController(IUnitOfWork UnitOfWork)
		{
			_UnitOfWork = UnitOfWork;
		}
		public IActionResult Index()
		{
			List<Category> ObjCategoyList = _UnitOfWork.Category.GetAll().ToList();
			
			return View(ObjCategoyList);
		}
		public IActionResult Create()
		{
			return View();
		}

		[HttpPost]
		public IActionResult Create(Category obj)
		{
			if (obj.Name == obj.DisplayOrder.ToString())
			{
				ModelState.AddModelError("name", "the DisplayOrder cannot ");
			}
			if (ModelState.IsValid)
			{
				_UnitOfWork.Category.Add(obj);
				_UnitOfWork.Save();
				TempData["Succes"] = "Category created successfully";
				return RedirectToAction("Index");
			}
			return View();
		}

		public IActionResult Edit(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}
			//Category? category = _db.Categories.Where(c => c.Id == id).FirstOrDefault();

			Category? category = _UnitOfWork.Category.Get(c => c.Id == id);
			//Category category = _db.Categories.Find(id);

			if (category == null)
			{
				return NotFound();
			}
			return View(category);
		}

		[HttpPost]
		public IActionResult Edit(Category obj)
		{

			if (ModelState.IsValid)
			{
				_UnitOfWork.Category.Update(obj);
				_UnitOfWork.Save();
				TempData["Succes"] = "Category Edited successfully";

				return RedirectToAction("Index");
			}
			return View();
		}
		public IActionResult Delete(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}
			//Category? category = _db.Categories.Where(c => c.Id == id).FirstOrDefault();

			Category? category = _UnitOfWork.Category.Get(c => c.Id == id);
			//Category category = _db.Categories.Find(id);

			if (category == null)
			{
				return NotFound();
			}
			return View(category);
		}

		[HttpPost, ActionName("Delete")]
		public IActionResult DeletePost(int? id)
		{

			Category obj = _UnitOfWork.Category.Get(c => c.Id == id);
			if (obj == null)
			{
				return NotFound();
			}
			_UnitOfWork.Category.Remove(obj);
			_UnitOfWork.Save();
			TempData["Succes"] = "Category Deleteed successfully";
			return RedirectToAction("Index");

		}
	}
}