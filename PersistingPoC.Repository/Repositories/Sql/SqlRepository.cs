using Microsoft.EntityFrameworkCore;
using PersistingPoC.Repository.Interfaces.Sql;
using PersistingPoC.Repository.Models.Sql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace PersistingPoC.Repository.Repositories.Sql
{
    public class SqlRepository<T> : ISqlRepository<T> where T : class
    {
        private readonly DbContextOptions _options;

        public SqlRepository(DbContextOptions options)
        {
            _options = options;
        }

        public virtual void Delete(T entity)
        {
            using var context = new SqlServerDbContext(_options);
            context.Set<T>().Remove(entity);
            context.SaveChanges();
        }

        public virtual async Task DeleteAsync(T entity)
        {
            await using var context = new SqlServerDbContext(_options);
            context.Set<T>().Remove(entity);
            await context.SaveChangesAsync();
        }

        public virtual IList<T> GetAll()
        {
            using var context = new SqlServerDbContext(_options);
            return context.Set<T>().ToList();
        }

        public virtual List<T> GetAll(int skip, int take, Expression<Func<T, object>> orderby, params Expression<Func<T, bool>>[] filters)
        {
            using var context = new SqlServerDbContext(_options);
            IQueryable<T> set = context.Set<T>();
            set = filters.Aggregate(set, (current, filter) => current.Where(filter));

            var ordered = ObjectSort(set, orderby);
            return ordered.Skip(skip).Take(take).ToList();
        }

        public virtual IList<T> GetAll(Expression<Func<T, bool>> expression)
        {
            using var context = new SqlServerDbContext(_options);
            return context.Set<T>().Where(expression).ToList();
        }

        public virtual async Task<IList<T>> GetAllAsync()
        {
            await using var context = new SqlServerDbContext(_options);
            return await context.Set<T>().ToListAsync();
        }

        public virtual async Task<List<T>> GetAllAsync(int skip, int take, Expression<Func<T, object>> orderby, params Expression<Func<T, bool>>[] filters)
        {
            await using var context = new SqlServerDbContext(_options);
            IQueryable<T> set = context.Set<T>();
            set = filters.Aggregate(set, (current, filter) => current.Where(filter));

            var ordered = ObjectSort(set, orderby);
            return await ordered.Skip(skip).Take(take).ToListAsync();
        }

        public virtual async Task<IList<T>> GetAllAsync(Expression<Func<T, bool>> expression)
        {
            await using var context = new SqlServerDbContext(_options);
            return await context.Set<T>().Where(expression).ToListAsync();
        }

        public virtual T GetById(int id)
        {
            using var context = new SqlServerDbContext(_options);
            return context.Set<T>().Find(id);
        }

        public virtual async Task<T> GetByIdAsync(int id)
        {
            await using var context = new SqlServerDbContext(_options);
            return await context.Set<T>().FindAsync(id);
        }

        public virtual void Insert(T entity)
        {
            using var context = new SqlServerDbContext(_options);
            context.Set<T>().Add(entity);
            context.SaveChanges();
        }

        public virtual async Task InsertAsync(T entity)
        {
            await using var context = new SqlServerDbContext(_options);
            await context.Set<T>().AddAsync(entity);
            await context.SaveChangesAsync();
        }

        public virtual void Update(T entity)
        {
            using var context = new SqlServerDbContext(_options);
            context.Entry(entity).State = EntityState.Modified;
            context.SaveChanges();
        }

        public virtual async Task UpdateAsync(T entity)
        {
            await using var context = new SqlServerDbContext(_options);
            context.Entry(entity).State = EntityState.Modified;
            await context.SaveChangesAsync();
        }

        protected IOrderedQueryable<TT> ObjectSort<TT>(IQueryable<TT> entities, Expression<Func<TT, object>> expression)
        {
            if (!(expression.Body is UnaryExpression unaryExpression))
            {
                return entities.OrderBy(expression);
            }

            var propertyExpression = (MemberExpression)unaryExpression.Operand;
            var parameters = expression.Parameters;

            if (propertyExpression.Type == typeof(DateTime))
            {
                var newExpression = Expression.Lambda<Func<TT, DateTime>>(propertyExpression, parameters);
                return entities.OrderBy(newExpression);
            }


            if (propertyExpression.Type == typeof(DateTime?))
            {
                var newExpression = Expression.Lambda<Func<TT, DateTime?>>(propertyExpression, parameters);
                return entities.OrderBy(newExpression);
            }

            if (propertyExpression.Type == typeof(int))
            {
                var newExpression = Expression.Lambda<Func<TT, int>>(propertyExpression, parameters);
                return entities.OrderBy(newExpression);
            }

            if (propertyExpression.Type == typeof(long))
            {
                var newExpression = Expression.Lambda<Func<TT, long>>(propertyExpression, parameters);
                return entities.OrderBy(newExpression);
            }

            throw new NotSupportedException("Object type resolution not implemented for this type");
        }
    }
}
