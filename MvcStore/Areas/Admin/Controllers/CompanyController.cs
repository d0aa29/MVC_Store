

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
    public class CompanyController : Controller
    {
        private readonly IUnitOfWork _UnitOfWork;

        public CompanyController(IUnitOfWork UnitOfWork)
        {
            _UnitOfWork = UnitOfWork;

        }
        public IActionResult Index()
        {
            List<Company> ObjCompanyList = _UnitOfWork.company.GetAll().ToList();


            return View(ObjCompanyList);
        }
        public IActionResult Upsert(int? id)
        {

            if (id == null || id == 0)
                return View(new Company());
            else
            {
                Company companyobj = _UnitOfWork.company.Get(u => u.Id == id);
                return View(companyobj);
            }
        }

        [HttpPost]

        public IActionResult Upsert(Company companyobj)
        {
            //if (obj.Name == obj.DisplayOrder.ToString())
            //{
            //    ModelState.AddModelError("name", "the DisplayOrder cannot ");
            //}
            if (ModelState.IsValid)
            {

                if (companyobj.Id == 0)
                    _UnitOfWork.company.Add(companyobj);
                else
                    _UnitOfWork.company.Update(companyobj);
                _UnitOfWork.Save();
                TempData["Succes"] = "Company created successfully";
                return RedirectToAction("Index");
            }
            else
            {
                return View(companyobj);
            }

        }

        //public IActionResult Edit(int? id)
        //{
        //	if (id == null)
        //	{
        //		return NotFound();
        //	}
        //	//Category? category = _db.Categories.Where(c => c.Id == id).FirstOrDefault();

        //	Company? Company = _UnitOfWork.Company.Get(c => c.Id == id);
        //	//Category category = _db.Categories.Find(id);

        //	if (Company == null)
        //	{
        //		return NotFound();
        //	}
        //	return View(Company);
        //}

        //[HttpPost]
        //public IActionResult Edit(Company obj)
        //{

        //	if (ModelState.IsValid)
        //	{
        //		_UnitOfWork.Company.Update(obj);
        //		_UnitOfWork.Save();
        //		TempData["Succes"] = "Company Edited successfully";

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

        //	Company? Company = _UnitOfWork.Company.Get(c => c.Id == id);
        //	//Category category = _db.Categories.Find(id);

        //	if (Company == null)
        //	{
        //		return NotFound();
        //	}
        //	return View(Company);
        //}

        //[HttpPost, ActionName("Delete")]
        //public IActionResult DeletePost(int? id)
        //{

        //	Company obj = _UnitOfWork.Company.Get(c => c.Id == id);
        //	if (obj == null)
        //	{
        //		return NotFound();
        //	}
        //	_UnitOfWork.Company.Remove(obj);
        //	_UnitOfWork.Save();
        //	TempData["Succes"] = "Company Deleteed successfully";
        //	return RedirectToAction("Index");

        //}

        [HttpGet]

        public IActionResult GetAll()
        {
            List<Company> ObjCompanyList = _UnitOfWork.company.GetAll().ToList();

            return Json(new { data = ObjCompanyList });
        }

        [HttpDelete]

        public IActionResult Delete(int? id)
        {

            var CompanyToDeleted = _UnitOfWork.company.Get(c => c.Id == id);
            if (CompanyToDeleted == null)
            {
                return Json(new { success = false, Message = "Error while deleting" });
            }



            _UnitOfWork.company.Remove(CompanyToDeleted);
            _UnitOfWork.Save();
            //List<Company> ObjCompanyList = _UnitOfWork.company.GetAll(includeproperties: "Category").ToList();

            return Json(new { success = true, Message = "deleting successfully" });
        }
    }

}


