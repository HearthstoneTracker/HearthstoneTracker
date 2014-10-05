// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogCommandBarViewModel.cs" company="">
//   
// </copyright>
// <summary>
//   The tests command bar view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Features.Diagnostics.Tests
{
    using System;
    using System.ComponentModel.Composition;

    using Caliburn.Micro;

    using HearthCap.Data;
    using HearthCap.Features.Games.Balloons;
    using HearthCap.Features.Games.Models;
    using HearthCap.Shell.TrayIcon;
    using HearthCap.Shell.WindowCommands;

#if DEBUG

    // [Export(typeof(IWindowCommand))]
#endif

    /// <summary>
    /// The tests command bar view model.
    /// </summary>
    public class TestsCommandBarViewModel : WindowCommandViewModel
    {
        /// <summary>
        /// The events.
        /// </summary>
        private readonly IEventAggregator events;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestsCommandBarViewModel"/> class.
        /// </summary>
        /// <param name="events">
        /// The events.
        /// </param>
        [ImportingConstructor]
        public TestsCommandBarViewModel(IEventAggregator events)
        {
            this.Order = -6;
            this.events = events;
            this.events.Subscribe(this);
        }

        /// <summary>
        /// The show balloon.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// </exception>
        public void ShowBalloon()
        {
            var gameResult = new GameResult { Hero = new Hero("mage") { Name = "Mage" }, OpponentHero = new Hero("mage") { Name = "Mage" } };
            gameResult.Victory = true;
            var title = "New game tracked.";
            var vm = IoC.Get<GameResultBalloonViewModel>();
            vm.SetGameResult(gameResult.ToModel());
            this.events.PublishOnBackgroundThread(new TrayNotification(title, vm, 10000));
            throw new ArgumentNullException();
        }
    }
}