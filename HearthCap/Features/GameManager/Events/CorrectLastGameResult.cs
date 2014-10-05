// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CorrectLastGameResult.cs" company="">
//   
// </copyright>
// <summary>
//   The correct last game result.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Features.GameManager.Events
{
    using System;

    /// <summary>
    /// The correct last game result.
    /// </summary>
    public class CorrectLastGameResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CorrectLastGameResult"/> class.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        public CorrectLastGameResult(Guid id)
        {
            this.Id = id;
        }

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the won.
        /// </summary>
        public bool? Won { get; set; }
    }
}