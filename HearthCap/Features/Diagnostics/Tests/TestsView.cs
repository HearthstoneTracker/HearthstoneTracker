// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestsView.cs" company="">
//   
// </copyright>
// <summary>
//   The tests view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Features.Diagnostics.Tests
{
    using System.ComponentModel.Composition;

    using Caliburn.Micro;

    using HearthCap.Core.GameCapture;
    using HearthCap.Logger.Interface;
    using HearthCap.Shell.Tabs;

#if DEBUG

    /// <summary>
    /// The tests view model.
    /// </summary>
    [Export(typeof(ITab))]
#endif
    public class TestsViewModel : TabViewModel, 
        IHandle<RequestDecksResponse>
    {
        /// <summary>
        /// The events.
        /// </summary>
        private readonly IEventAggregator events;

        /// <summary>
        /// The capture engine.
        /// </summary>
        private readonly ICaptureEngine captureEngine;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestsViewModel"/> class.
        /// </summary>
        /// <param name="events">
        /// The events.
        /// </param>
        /// <param name="captureEngine">
        /// The capture engine.
        /// </param>
        [ImportingConstructor]
        public TestsViewModel(IEventAggregator events, 
            ICaptureEngine captureEngine)
        {
            this.DisplayName = "Tests";
            this.events = events;
            this.captureEngine = captureEngine;
            events.Subscribe(this);
        }

        /// <summary>
        /// The get decks.
        /// </summary>
        public void GetDecks()
        {
            this.events.PublishOnBackgroundThread(new RequestDecks());
        }

        /// <summary>
        /// Handles the message.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public void Handle(RequestDecksResponse message)
        {
            int dummy;
            var deckids = message.DeckIds.ToArray();
        }
    }
}