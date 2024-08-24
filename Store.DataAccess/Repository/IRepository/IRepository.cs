using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Store.DataAccess.Repository.IRepository
{
	public interface IRepository<T> where T : class
	{
		// T-Category
		IEnumerable<T> GetAll(Expression<Func<T, bool>>? Filter = null, string? includeproperties =null);
		T Get(Expression<Func<T, bool>> Filter, string? includeproperties=null,bool tracked=false);
		void Add(T entity);
		void Remove(T entity);
		void RemoveRange(IEnumerable<T> entity);
	
	}
}
