// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SelectedArenaSessionChanged.cs" company="">
//   
// </copyright>
// <summary>
//   The selected arena session changed.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Features.ArenaSessions
{
    using System;

    /// <summary>
    /// The selected arena session changed.
    /// </summary>
    public class SelectedArenaSessionChanged
    {
        /// <summary>
        /// Gets or sets the source.
        /// </summary>
        public object Source { get; protected set; }

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        public Guid? Id { get; protected set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectedArenaSessionChanged"/> class.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="id">
        /// The id.
        /// </param>
        public SelectedArenaSessionChanged(object source, Guid? id)
        {
            this.Source = source;
            this.Id = id;
        }
    }
}