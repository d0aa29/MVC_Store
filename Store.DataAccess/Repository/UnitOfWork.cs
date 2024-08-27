using Store.Models;
using Store.DataAccess.Data;
using Store.DataAccess.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _db;
        public ICategoryRepository Category { get; private set; }
        public IProductRepository Product { get; private set; }
        public ICompanyRepository company { get; private set; }

        public IShoppingCart ShoppingCart { get; private set; }

        public IAppUserRepository appUser { get; private set; }

        public IOrderHeaderRepository orderHeader { get; private set; }

        public IOrderDetailRepository orderDetail { get; private set; }

      
        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
            Category=new CategoryRepository(_db);
            Product=new ProductRepository(_db);
            company=new CompanyRepository(_db);
            ShoppingCart=new ShoppingCartRepository(_db);
            appUser=new AppUserRepository(_db);
            orderHeader=new OrderHeaderRepository(_db);
            orderDetail= new OrderDetailRepository(_db);

        }

        public void Save()
        {
            _db.SaveChanges();
        }
       

   
    }
}
