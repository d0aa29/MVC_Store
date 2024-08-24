

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using Store.DataAccess.Data;
using Store.DataAccess.Repository;
using Store.DataAccess.Repository.IRepository;
using Store.Models;
using Store.Models.ViewModel;
using Store.Utility;


namespace Mystore.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _UnitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductController(IUnitOfWork UnitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _UnitOfWork = UnitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            List<Product> ObjProductList = _UnitOfWork.Product.GetAll(includeproperties: "Category").ToList();


            return View(ObjProductList);
        }
        public IActionResult Upsert(int? id)
        {
            IEnumerable<SelectListItem> mycategorylist = _UnitOfWork.Category
                .GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                });
            ViewBag.mycategorylist = mycategorylist;
            ProductVM productVM = new()
            {
                Product = new Product(),
                mycategorylist = mycategorylist

            };
            if (id == null || id == 0)
                return View(productVM);
            else
            {
                productVM.Product = _UnitOfWork.Product.Get(u => u.Id == id);
                return View(productVM);
            }
        }

        [HttpPost]

        public IActionResult Upsert(ProductVM obj, IFormFile? formFile)
        {
            //if (obj.Name == obj.DisplayOrder.ToString())
            //{
            //    ModelState.AddModelError("name", "the DisplayOrder cannot ");
            //}
            if (ModelState.IsValid)
            {

                string wwwRootPath = _webHostEnvironment.WebRootPath;
                if (formFile != null)
                {

                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(formFile.FileName);
                    string productpath = Path.Combine(wwwRootPath, @"images\product");

                    if (!(string.IsNullOrEmpty(obj.Product.ImageUrl)))
                    {

                        var oldpath = Path.Combine(wwwRootPath, obj.Product.ImageUrl.TrimStart('\\'));

                        if (System.IO.File.Exists(oldpath))
                        {
                            System.IO.File.Delete(oldpath);
                        }
                    }

                    using (var filestrem = new FileStream(Path.Combine(productpath, fileName), FileMode.Create))
                    {
                        formFile.CopyTo(filestrem);
                    }

                    obj.Product.ImageUrl = @"\images\product\" + fileName;
                }
                if (obj.Product.Id == 0)
                    _UnitOfWork.Product.Add(obj.Product);
                else
                    _UnitOfWork.Product.Update(obj.Product);
                _UnitOfWork.Save();
                TempData["Succes"] = "Product created successfully";
                return RedirectToAction("Index");
            }
            else
            {
                obj.mycategorylist = _UnitOfWork.Category
                .GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                });
                return View(obj);
            }

        }

        //public IActionResult Edit(int? id)
        //{
        //	if (id == null)
        //	{
        //		return NotFound();
        //	}
        //	//Category? category = _db.Categories.Where(c => c.Id == id).FirstOrDefault();

        //	Product? Product = _UnitOfWork.Product.Get(c => c.Id == id);
        //	//Category category = _db.Categories.Find(id);

        //	if (Product == null)
        //	{
        //		return NotFound();
        //	}
        //	return View(Product);
        //}

        //[HttpPost]
        //public IActionResult Edit(Product obj)
        //{

        //	if (ModelState.IsValid)
        //	{
        //		_UnitOfWork.Product.Update(obj);
        //		_UnitOfWork.Save();
        //		TempData["Succes"] = "Product Edited successfully";

        //		return RedirectToAction("Index");
        //	}
        //	return View();
        //}
        //public IActionResult Delete(int? id)
        //{
        //	if (id == null)
        //	{
        //		return NotFound();
        //	}
        //	//Category? category = _db.Categories.Where(c => c.Id == id).FirstOrDefault();

        //	Product? Product = _UnitOfWork.Product.Get(c => c.Id == id);
        //	//Category category = _db.Categories.Find(id);

        //	if (Product == null)
        //	{
        //		return NotFound();
        //	}
        //	return View(Product);
        //}

        //[HttpPost, ActionName("Delete")]
        //public IActionResult DeletePost(int? id)
        //{

        //	Product obj = _UnitOfWork.Product.Get(c => c.Id == id);
        //	if (obj == null)
        //	{
        //		return NotFound();
        //	}
        //	_UnitOfWork.Product.Remove(obj);
        //	_UnitOfWork.Save();
        //	TempData["Succes"] = "Product Deleteed successfully";
        //	return RedirectToAction("Index");

        //}

        [HttpGet]

        public IActionResult GetAll()
        {
            List<Product> ObjProductList = _UnitOfWork.Product.GetAll(includeproperties: "Category").ToList();

            return Json(new { data = ObjProductList });
        }

        [HttpDelete]

        public IActionResult Delete(int? id)
        {

            var ProductToDeleted = _UnitOfWork.Product.Get(c => c.Id == id);
            if (ProductToDeleted == null)
            {
                return Json(new { success = false, Message = "Error while deleting" });
            }


            var oldpath = Path.Combine(_webHostEnvironment.WebRootPath,
                          ProductToDeleted.ImageUrl.TrimStart('\\'));

            if (System.IO.File.Exists(oldpath))
            {
                System.IO.File.Delete(oldpath);
            }
            _UnitOfWork.Product.Remove(ProductToDeleted);
            _UnitOfWork.Save();
            List<Product> ObjProductList = _UnitOfWork.Product.GetAll(includeproperties: "Category").ToList();

            return Json(new { success = true, Message = "deleting successfully" });
        }
    }

}


