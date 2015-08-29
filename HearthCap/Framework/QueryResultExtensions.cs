using System.Linq;

namespace HearthCap.Framework
{
    public static class QueryResultExtensions
    {
        public static QueryResult<TEntity> AsResult<TEntity>(this IQueryable<TEntity> query) where TEntity : class
        {
            return new QueryResult<TEntity>(query);
        }
    }
}
