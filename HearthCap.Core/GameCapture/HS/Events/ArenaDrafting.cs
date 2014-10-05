// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ArenaDrafting.cs" company="">
//   
// </copyright>
// <summary>
//   The arena drafting.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Core.GameCapture.HS
{
    using HearthCap.Core.GameCapture.HS.Events;

    /// <summary>
    /// The arena drafting.
    /// </summary>
    public class ArenaDrafting : GameEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ArenaDrafting"/> class.
        /// </summary>
        public ArenaDrafting()
            : base("Arena: drafting cards")
        {
        }
    }
}