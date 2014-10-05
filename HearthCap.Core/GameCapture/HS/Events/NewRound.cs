// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NewRound.cs" company="">
//   
// </copyright>
// <summary>
//   The new round.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Core.GameCapture.HS.Events
{
    /// <summary>
    /// The new round.
    /// </summary>
    public class NewRound : GameEvent
    {
        /// <summary>
        /// Gets or sets the current.
        /// </summary>
        public int Current { get; protected set; }

        /// <summary>
        /// Gets or sets a value indicating whether my turn.
        /// </summary>
        public bool MyTurn { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="NewRound"/> class.
        /// </summary>
        /// <param name="current">
        /// The current.
        /// </param>
        /// <param name="myTurn">
        /// The my turn.
        /// </param>
        public NewRound(int current, bool myTurn = false)
            : base(string.Format("New game round (current: {0})", current))
        {
            this.Current = current;
            this.MyTurn = myTurn;
        }
    }
}