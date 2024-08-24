using Store.DataAccess.Data;
using Store.DataAccess.Repository.IRepository;
using Store.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.DataAccess.Repository
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        private readonly ApplicationDbContext _db;
        public ProductRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Product product)
        {
            //_db.Products.Update(product);
            //or
            var objfromdb = _db.Products.FirstOrDefault(p => p.Id == product.Id);
            if (objfromdb != null) {
				objfromdb.ISBN= product.ISBN;
				objfromdb.ListPrice= product.ListPrice;
                objfromdb.Price= product.Price;
				objfromdb.Author= product.Author;
                objfromdb.Description= product.Description;
          
				objfromdb.CategoryId= product.CategoryId;
				objfromdb.Title= product.Title;
				objfromdb.Price50= product.Price50;
				objfromdb.Price100= product.Price100;

                if (product.ImageUrl != null) {
					objfromdb.ImageUrl = product.ImageUrl;

				}


			}

        }
    }
}
