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
    public class OrderDetailRepositorytory : Repository<OrderDetail> , IOrderDetailRepository
    {
		private readonly ApplicationDbContext _db;
		public OrderDetailRepositorytory(ApplicationDbContext db) : base(db)
		{
			_db = db;
		}

		//public void Save()
		//{
		//	_db.SaveChanges();
		//}

		public void Update(OrderDetail obj)
		{
			_db.OrderDetails.Update(obj);
		}
	}
}
