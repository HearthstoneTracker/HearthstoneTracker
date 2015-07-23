namespace HearthCap.Features.Diagnostics.Tests
{
    using System.ComponentModel.Composition;

    using Caliburn.Micro;

    using HearthCap.Core.GameCapture;
    using HearthCap.Logger.Interface;
    using HearthCap.Shell.Tabs;

#if DEBUG
    [Export(typeof(ITab))]
#endif
    public class TestsViewModel : TabViewModel,
        IHandle<RequestDecksResponse>
    {
        private readonly IEventAggregator events;

        private readonly ICaptureEngine captureEngine;

        [ImportingConstructor]
        public TestsViewModel(IEventAggregator events,
            ICaptureEngine captureEngine)
        {
            DisplayName = "Tests";
            this.events = events;
            this.captureEngine = captureEngine;
            events.Subscribe(this);
        }

        public void GetDecks()
        {
            events.PublishOnBackgroundThread(new RequestDecks());
        }

        /// <summary>
        /// Handles the message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Handle(RequestDecksResponse message)
        {
            // int dummy;
            // var deckids = message.DeckIds.ToArray();
        }
    }
}