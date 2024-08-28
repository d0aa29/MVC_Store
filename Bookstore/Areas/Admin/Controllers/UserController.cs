

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using Store.DataAccess.Data;
using Store.DataAccess.Repository;
using Store.DataAccess.Repository.IRepository;
using Store.Models;
using Store.Models.ViewModel;
using Store.Utility;
using Microsoft.AspNetCore.Identity;


namespace Mystore.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class UserController : Controller
    {
        private readonly IUnitOfWork _UnitOfWork;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public UserController(UserManager<IdentityUser> userManager,IUnitOfWork UnitOfWork, RoleManager<IdentityRole> roleManager)
        {
            _UnitOfWork = UnitOfWork;
            _userManager = userManager;
            _roleManager = roleManager;

        }
        public IActionResult Index()
        {
           

            return View();
        }
     

        [HttpGet]

        public IActionResult GetAll()
        {
            List<AppUser> objUserList = _UnitOfWork.appUser.GetAll(includeproperties:"Company").ToList();

        

            foreach (var user in objUserList) {
              user.Role = _userManager.GetRolesAsync(user).GetAwaiter().GetResult().FirstOrDefault();

            }

            return Json(new { data = objUserList });
        }

        [HttpPost]

        public IActionResult LockUnlock([FromBody]string id)
        {

           var objFromDb=_UnitOfWork.appUser.Get(u=>u.Id == id);
            if (objFromDb == null) {
                return Json(new { success = false, message = "Error during Locking/Unlocking " });
            }
            if (objFromDb.LockoutEnd != null && objFromDb.LockoutEnd > DateTime.Now)
            {
                //user is currently locked and we need to unlock them
                objFromDb.LockoutEnd = DateTime.Now;
            }
            else
            {
                objFromDb.LockoutEnd = DateTime.Now.AddYears(1000);
            }
            _UnitOfWork.appUser.Update(objFromDb);
            _UnitOfWork.Save();
            return Json(new { success = true, message = " Locking/Unlocking successfully" });
        }

        public IActionResult RoleManagment(string userId)
        {
            RoleManagmentVM RoleVM = new RoleManagmentVM()
            {
                ApplicationUser = _UnitOfWork.appUser.Get(u => u.Id == userId, includeproperties: "Company"),
                RoleList = _roleManager.Roles.Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Name
                }),
                CompanyList = _UnitOfWork.company.GetAll().Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                }),
            };

            RoleVM.ApplicationUser.Role = _userManager.GetRolesAsync(_UnitOfWork.appUser.Get(u => u.Id == userId))
                    .GetAwaiter().GetResult().FirstOrDefault();
            return View(RoleVM);
            //Json(new { success = true, message = " Role Updated successfully" });
        }

        [HttpPost]
        public IActionResult RoleManagment(RoleManagmentVM roleManagmentVM) 
		{
			string oldRole = _userManager.GetRolesAsync(_UnitOfWork.appUser.Get(u => u.Id == roleManagmentVM.ApplicationUser.Id))
					.GetAwaiter().GetResult().FirstOrDefault();

			AppUser applicationUser = _UnitOfWork.appUser.Get(u => u.Id == roleManagmentVM.ApplicationUser.Id);
			if (!(roleManagmentVM.ApplicationUser.Role == oldRole))
			{
				//a role was updated
				if (roleManagmentVM.ApplicationUser.Role == SD.Role_Company)
				{
					applicationUser.CompanyId = roleManagmentVM.ApplicationUser.CompanyId;
				}
				if (oldRole == SD.Role_Company)
				{
					applicationUser.CompanyId = null;
				}
				_UnitOfWork.appUser.Update(applicationUser);
				_UnitOfWork.Save();

				_userManager.RemoveFromRoleAsync(applicationUser, oldRole).GetAwaiter().GetResult();
				_userManager.AddToRoleAsync(applicationUser, roleManagmentVM.ApplicationUser.Role).GetAwaiter().GetResult();

			}
			else
			{
				if (oldRole == SD.Role_Company && applicationUser.CompanyId != roleManagmentVM.ApplicationUser.CompanyId)
				{
					applicationUser.CompanyId = roleManagmentVM.ApplicationUser.CompanyId;
					_UnitOfWork.appUser.Update(applicationUser);
					_UnitOfWork.Save();
				}
			}

			return RedirectToAction("Index");

		}

	}

}


