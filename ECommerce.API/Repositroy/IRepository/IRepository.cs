﻿using System.Linq.Expressions;

namespace ECommerce.API.Repositroy.IRepository
{
    public interface IRepository<T> where T : class
    {
        public void Create(T entity);

        public void Create(IEnumerable<T> entities);

        public void Edit(T entity);

        public void Delete(T entity);

        public void Delete(IEnumerable<T> entities);

        public void Commit();

        public IEnumerable<T> Get(Expression<Func<T, bool>>? filter = null, Expression<Func<T, object>>[]? includes = null, bool tracked = true);

        public T? GetOne(Expression<Func<T, bool>>? filter = null, Expression<Func<T, object>>[]? includes = null, bool tracked = true);
    }
}
