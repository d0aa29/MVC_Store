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
    public class ProductImageRepository : Repository<ProductImage> , IProductImageRepository
	{
		private readonly ApplicationDbContext _db;
		public ProductImageRepository(ApplicationDbContext db) : base(db)
		{
			_db = db;
		}

		//public void Save()
		//{
		//	_db.SaveChanges();
		//}

		public void Update(ProductImage obj)
		{
			_db.ProductImages.Update(obj);
		}

		
	}
}
