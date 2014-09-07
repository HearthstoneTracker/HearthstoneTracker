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
    public class TestsCommandBarViewModel : WindowCommandViewModel
    {
        private readonly IEventAggregator events;

        [ImportingConstructor]
        public TestsCommandBarViewModel(IEventAggregator events)
        {
            this.Order = -6;
            this.events = events;
            this.events.Subscribe(this);
        }

        public void ShowBalloon()
        {
            var gameResult = new GameResult()
                                 {
                                     Hero = new Hero("mage")
                                                {
                                                    Name = "Mage"
                                                },
                                     OpponentHero = new Hero("mage")
                                     {
                                         Name = "Mage"
                                     }
                                 };
            gameResult.Victory = true;
            var title = "New game tracked.";
            var vm = IoC.Get<GameResultBalloonViewModel>();
            vm.SetGameResult(gameResult.ToModel());
            events.PublishOnBackgroundThread(new TrayNotification(title, vm, 10000));
            throw new ArgumentNullException();
        }
    }
}