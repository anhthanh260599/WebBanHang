using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using WebBanHangOnline.Models.UnitOfWork;

namespace WebBanHangOnline.Models.Repository
{
    public class GenericRepository<T> : IRepository<T> where T : class
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly ApplicationDbContext db;

        public GenericRepository( IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
            this.db = new ApplicationDbContext();
        }

        public IEnumerable<T> GetAll()
        {
            var propertyName = "Id"; // Assume that the property name is "Id"
            var parameter = Expression.Parameter(typeof(T), "x");
            var property = Expression.Property(parameter, propertyName);


            var lambda = Expression.Lambda<Func<T, int>>(property, parameter);

            return db.Set<T>().OrderByDescending(lambda).ToList();
        }

        public void Add(T entity)
        {
            //db.Set<T>().Add(entity);
            _unitOfWork.DbContext.Set<T>().Add(entity);
        }
    }
}