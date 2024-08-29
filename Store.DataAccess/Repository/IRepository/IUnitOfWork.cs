using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.DataAccess.Repository.IRepository
{
    public interface IUnitOfWork
    {
        public ICategoryRepository Category { get; }
        public IProductRepository Product { get; }
        public ICompanyRepository company { get; }
        public IShoppingCart ShoppingCart { get; }
        public IAppUserRepository appUser { get; }
        public IOrderHeaderRepository orderHeader { get; }
        public IOrderDetailRepository orderDetail { get; }

        public IProductImageRepository productImage { get; }

		void Save();
    }
}
