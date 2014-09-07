namespace HearthCap.Features.Games.Balloons
{
    using System;
    using System.ComponentModel.Composition;
    using System.Linq;

    using Caliburn.Micro;

    using HearthCap.Core.GameCapture.HS.Events;
    using HearthCap.Data;
    using HearthCap.Features.GameManager;
    using HearthCap.Features.GameManager.Events;
    using HearthCap.Features.Games.Models;

    [Export(typeof(GameStartedBalloonViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class GameStartedBalloonViewModel : PropertyChangedBase
    {
        private readonly IEventAggregator events;

        private readonly GameManager gameManager;

        private readonly IRepository<Hero> heroRepository;

        private bool hasCoin;

        private Hero opponentHero;

        private Hero hero;

        private GameMode gameMode;

        [ImportingConstructor]
        public GameStartedBalloonViewModel(IEventAggregator events, GameManager gameManager, IRepository<Hero> heroRepository)
        {
            this.events = events;
            this.gameManager = gameManager;
            this.heroRepository = heroRepository;
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

        public bool HasCoin
        {
            get
            {
                return this.hasCoin;
            }
            set
            {
                if (value.Equals(this.hasCoin))
                {
                    return;
                }
                this.hasCoin = value;
                this.NotifyOfPropertyChange(() => this.HasCoin);
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

        public void SetGameResult(GameStarted gameResult)
        {
            if (gameResult == null)
            {
                throw new ArgumentNullException("gameResult");
            }

            this.GameMode = gameResult.GameMode;
            this.Hero = heroRepository.Query(q => q.FirstOrDefault(x => x.Key == gameResult.Hero));
            this.OpponentHero = heroRepository.Query(q => q.FirstOrDefault(x => x.Key == gameResult.OpponentHero));
        }
    }
}
