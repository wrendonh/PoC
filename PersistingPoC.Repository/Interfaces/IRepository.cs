using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace PersistingPoC.Repository.Interfaces
{
    public interface IRepository<T> where T : class
    {        
        IList<T> GetAll();
        List<T> GetAll(int skip, int take, Expression<Func<T, object>> orderby, params Expression<Func<T, bool>>[] filters);
        IList<T> GetAll(Expression<Func<T, bool>> expression);
        Task<IList<T>> GetAllAsync();
        void Insert(T entity);
        Task InsertAsync(T entity);
        void Update(T entity);
        Task UpdateAsync(T entity);
        void Delete(T entity);
        Task DeleteAsync(T entity);
    }
}
