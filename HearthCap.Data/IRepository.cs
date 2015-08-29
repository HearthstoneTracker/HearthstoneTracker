using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace HearthCap.Data
{
    public interface IRepository<T>
        where T : class
    {
        List<T> ToList(Func<IQueryable<T>, IQueryable<T>> query);

        Task<List<T>> ToListAsync(Func<IQueryable<T>, IQueryable<T>> query);

        Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> query);

        Task SaveOrUpdateAsync(T entity, object id);

        Task Delete(object id);

        Task<TReturn> QueryAsync<TReturn>(Func<IQueryable<T>, Task<TReturn>> query);

        TReturn Query<TReturn>(Func<IQueryable<T>, TReturn> query);

        TReturn SqlQuery<TReturn>(string sql, params object[] parameters);

        T FirstOrDefault(Expression<Func<T, bool>> query);
    }
}
