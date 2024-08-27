using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Store.DataAccess.Data;
using Store.DataAccess.Repository;
using Store.DataAccess.Repository.IRepository;
using Store.Models;

namespace Store.DataAccess.Repository
{
	public class OrderHeaderRepository : Repository<OrderHeader>, IOrderHeaderRepository
	{
		private ApplicationDbContext _db;
		public OrderHeaderRepository(ApplicationDbContext db) : base(db)
		{
			_db = db;
		}



		public void Update(OrderHeader obj)
		{
			_db.OrderHeaders.Update(obj);
		}
		
//	public void UpdateStatus(int id, string orderStatus, string? paymentStatus = null)
//	{
//		var orderfromDb = _db.OrderHeaders.FirstOrDefault(u => u.Id == id);
//		if (orderfromDb != null)
//		{
//			orderfromDb.OrderStatus = orderStatus;
//			if (!string.IsNullOrEmpty(paymentStatus))
//			{
//				orderfromDb.PaymentStatus = paymentStatus;
//			}
//		}
//	}
		public void UpdateStatus(int id, string orderStatus, string? paymentStatus = null)
		{
			var orderFromDb = _db.OrderHeaders.FirstOrDefault(u => u.Id == id);
			if (orderFromDb != null)
			{
				orderFromDb.OrderStatus = orderStatus;
				if (!string.IsNullOrEmpty(paymentStatus))
				{
					orderFromDb.PaymentStatus = paymentStatus;
				}
			}
		}

		public void UpdateStripePaymentId(int id, string sessionId, string paymentIntentId)
		{
			var orderFromDb = _db.OrderHeaders.FirstOrDefault(u => u.Id == id);
			if (!string.IsNullOrEmpty(sessionId))
			{
				orderFromDb.SessionId = sessionId;
			}
			if (!string.IsNullOrEmpty(paymentIntentId))
			{
				orderFromDb.PaymentIntentId = paymentIntentId;
				orderFromDb.PaymentDate = DateTime.Now;
			}
		}
	}
}


//public class OrderHeaderRepository : Repository<OrderHeader>, IOrderHeaderRepository
//{
//	private readonly ApplicationDbContext _db;
//	public OrderHeaderRepository(ApplicationDbContext db) : base(db)
//	{
//		_db = db;
//	}

//	//public void Save()
//	//{
//	//	_db.SaveChanges();
//	//}

//	public void Update(OrderHeader obj)
//	{
//		_db.OrderHeaders.Update(obj);
//	}

//	public void UpdateStatus(int id, string orderStatus, string? paymentStatus = null)
//	{
//		var orderfromDb = _db.OrderHeaders.FirstOrDefault(u => u.Id == id);
//		if (orderfromDb != null)
//		{
//			orderfromDb.OrderStatus = orderStatus;
//			if (!string.IsNullOrEmpty(paymentStatus))
//			{
//				orderfromDb.PaymentStatus = paymentStatus;
//			}
//		}
//	}

//	public void UpdateStripePaymentId(int id, string sessionId, string paymentIntentId)
//	{
//		var orderfromDb = _db.OrderHeaders.FirstOrDefault(u => u.Id == id);
//		if (!string.IsNullOrEmpty(sessionId))
//		{
//			orderfromDb.PaymentStatus = sessionId;
//		}
//		if (!string.IsNullOrEmpty(paymentIntentId))
//		{
//			orderfromDb.PaymentStatus = paymentIntentId;
//			orderfromDb.OrderDate = DateTime.Now;
//		}
//	}
//}