namespace HearthCap.Features.Games.Balloons
{
    using System;
    using System.ComponentModel.Composition;

    using Caliburn.Micro;

    using HearthCap.Data;
    using HearthCap.Features.GameManager;
    using HearthCap.Features.GameManager.Events;
    using HearthCap.Features.Games.Models;

    [Export(typeof(GameResultBalloonViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class GameResultBalloonViewModel : PropertyChangedBase
    {
        private readonly IEventAggregator events;

        private readonly GameManager gameManager;

        private bool victory;

        private Hero opponentHero;

        private Hero hero;

        private GameResultModel gameResult;

        private GameMode gameMode;

        private int turns;

        [ImportingConstructor]
        public GameResultBalloonViewModel(IEventAggregator events, GameManager gameManager)
        {
            this.events = events;
            this.gameManager = gameManager;
        }

        public Hero Hero
        {
            get
            {
                return this.hero;
            }
            set
            {
                if (Equals(value, this.hero))
                {
                    return;
                }
                this.hero = value;
                this.NotifyOfPropertyChange(() => this.Hero);
            }
        }

        public Hero OpponentHero
        {
            get
            {
                return this.opponentHero;
            }
            set
            {
                if (Equals(value, this.opponentHero))
                {
                    return;
                }
                this.opponentHero = value;
                this.NotifyOfPropertyChange(() => this.OpponentHero);
            }
        }

        public bool Victory
        {
            get
            {
                return this.victory;
            }
            set
            {
                if (value.Equals(this.victory))
                {
                    return;
                }
                this.victory = value;
                this.NotifyOfPropertyChange(() => this.Victory);
            }
        }

        public GameMode GameMode
        {
            get
            {
                return this.gameMode;
            }
            set
            {
                if (value == this.gameMode)
                {
                    return;
                }
                this.gameMode = value;
                this.NotifyOfPropertyChange(() => this.GameMode);
            }
        }

        public int Turns
        {
            get
            {
                return this.turns;
            }
            set
            {
                if (value == this.turns)
                {
                    return;
                }
                this.turns = value;
                this.NotifyOfPropertyChange(() => this.Turns);
            }
        }

        public async void CorrectWin()
        {
            // events.PublishOnBackgroundThread(new CorrectLastGameResult(gameResult.Id){ Won = true });
            if (gameResult == null) return;

            gameResult.Victory = true;
            Victory = true;
            await gameManager.UpdateGame(gameResult);
        }

        public void SetGameResult(GameResultModel gameResult)
        {
            if (gameResult == null)
            {
                throw new ArgumentNullException("gameResult");
            }
            this.gameResult = gameResult;
            this.Hero = gameResult.Hero;
            this.OpponentHero = gameResult.OpponentHero;
            this.Victory = gameResult.Victory;
            this.GameMode = gameResult.GameMode;
            this.Turns = gameResult.Turns;
        }
    }
}
