using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Caliburn.Micro;

namespace HearthCap.Framework
{
    public class QueryResult<TEntity> : IResult
        where TEntity : class
    {
        public QueryResult(IQueryable<TEntity> query)
        {
            Query = query;
        }

        public event EventHandler<ResultCompletionEventArgs> Completed;

        public IQueryable<TEntity> Query { get; set; }

        public IList<TEntity> Result { get; private set; }

        public async void Execute(CoroutineExecutionContext context)
        {
            try
            {
                Result = await Query.ToListAsync();
                OnCompleted();
            }
            catch (Exception ex)
            {
                OnCompleted(ex);
            }
        }

        private void OnCompleted(Exception error = null, bool wasCancelled = false)
        {
            var handler = Completed;
            if (handler != null)
            {
                handler(this, new ResultCompletionEventArgs { Error = error, WasCancelled = wasCancelled });
            }
        }
    }
}
