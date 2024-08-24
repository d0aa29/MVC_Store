using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Store.DataAccess.Data;
using Store.DataAccess.Repository.IRepository;

namespace Store.DataAccess.Repository
{
	public class Repository<T> : IRepository<T> where T : class
	{
		private readonly ApplicationDbContext _db;
		internal DbSet<T> dbSet;
		public Repository(ApplicationDbContext db)
		{
			_db = db;
			this.dbSet = _db.Set<T>();
			// dbSet == _db.Categeories

		}
		public void Add(T entity)
		{
			dbSet.Add(entity);
		}

		

		public T Get(Expression<Func<T, bool>> Filter, string? includeproperties, bool tracked = false)
		{
			IQueryable<T> query;

            if (tracked)
			{
                query = dbSet;
				
			}
			else
			{
                query = dbSet.AsNoTracking(); 
            }
            query = query.Where(Filter);
            if (!string.IsNullOrEmpty(includeproperties))
            {
                foreach (var property in includeproperties
                    .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(property);
                }

            }

            return query.FirstOrDefault();
        }

		public IEnumerable<T> GetAll(Expression<Func<T, bool>>? Filter, string? includeproperties = null)
		{

			IQueryable<T> query = dbSet;
			if(Filter!=null)
            query = query.Where(Filter);
            if (!string.IsNullOrEmpty( includeproperties) )
			{
				foreach (var property in includeproperties
					.Split(new char[]{ ','} ,StringSplitOptions.RemoveEmptyEntries))
				{
					query=query.Include(property);
				}

			}
			return query;
	    }

		public void Remove(T entity)
		{
			dbSet.Remove(entity);
		}

		public void RemoveRange(IEnumerable<T> entity)
		{
			dbSet.RemoveRange(entity);
		}
	}
}
