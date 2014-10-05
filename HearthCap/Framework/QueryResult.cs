// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QueryResult.cs" company="">
//   
// </copyright>
// <summary>
//   The query result.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Framework
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;

    using Caliburn.Micro;

    /// <summary>
    /// The query result.
    /// </summary>
    /// <typeparam name="TEntity">
    /// </typeparam>
    public class QueryResult<TEntity> : IResult where TEntity : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QueryResult{TEntity}"/> class.
        /// </summary>
        /// <param name="query">
        /// The query.
        /// </param>
        public QueryResult(IQueryable<TEntity> query)
        {
            this.Query = query;
        }

        /// <summary>
        /// The completed.
        /// </summary>
        public event EventHandler<ResultCompletionEventArgs> Completed;

        /// <summary>
        /// Gets or sets the query.
        /// </summary>
        public IQueryable<TEntity> Query { get; set; }

        /// <summary>
        /// Gets the result.
        /// </summary>
        public IList<TEntity> Result { get; private set; }

        /// <summary>
        /// The execute.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        public async void Execute(CoroutineExecutionContext context)
        {
            try
            {
                this.Result = await this.Query.ToListAsync();
                this.OnCompleted();
            }
            catch (Exception ex)
            {
                this.OnCompleted(ex);
            }
        }

        /// <summary>
        /// The on completed.
        /// </summary>
        /// <param name="error">
        /// The error.
        /// </param>
        /// <param name="wasCancelled">
        /// The was cancelled.
        /// </param>
        private void OnCompleted(Exception error = null, bool wasCancelled = false)
        {
            var handler = this.Completed;
            if (handler != null)
            {
                handler(this, new ResultCompletionEventArgs { Error = error, WasCancelled = wasCancelled });
            }
        }
    }
}