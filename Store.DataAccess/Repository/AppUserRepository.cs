using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Store.DataAccess.Data;
using Store.DataAccess.Repository.IRepository;
using Store.Models;

namespace Store.DataAccess.Repository
{
    public class AppUserRepository : Repository<AppUser> , IAppUserRepository
    {
		private readonly ApplicationDbContext _db;
		public AppUserRepository(ApplicationDbContext db) : base(db)
		{
			_db = db;
		}



        //public void Save()
        //{
        //	_db.SaveChanges();
        //}

        public void Update(AppUser obj)
        {
            _db.AppUsers.Update(obj);
        }
    }
}
