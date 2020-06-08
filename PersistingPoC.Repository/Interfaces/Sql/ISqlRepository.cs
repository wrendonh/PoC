using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace PersistingPoC.Repository.Interfaces.Sql
{
    public interface ISqlRepository<T> : IRepository<T> where T : class 
    {
        T GetById(int id);
        Task<T> GetByIdAsync(int id);
        Task<List<T>> GetAllAsync(int skip, int take, Expression<Func<T, object>> orderby, params Expression<Func<T, bool>>[] filters);
        Task<IList<T>> GetAllAsync(Expression<Func<T, bool>> expression);
    }
}
