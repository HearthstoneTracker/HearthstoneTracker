using System;
using System.ComponentModel.Composition;
using System.Linq;
using Caliburn.Micro;
using HearthCap.Core.GameCapture.HS.Events;
using HearthCap.Data;

namespace HearthCap.Features.Games.Balloons
{
    [Export(typeof(GameStartedBalloonViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class GameStartedBalloonViewModel : PropertyChangedBase
    {
        private readonly IEventAggregator events;

        private readonly GameManager.GameManager gameManager;

        private readonly IRepository<Hero> heroRepository;

        private bool hasCoin;

        private Hero opponentHero;

        private Hero hero;

        private GameMode gameMode;

        [ImportingConstructor]
        public GameStartedBalloonViewModel(IEventAggregator events, GameManager.GameManager gameManager, IRepository<Hero> heroRepository)
        {
            this.events = events;
            this.gameManager = gameManager;
            this.heroRepository = heroRepository;
        }

        public Hero Hero
        {
            get { return hero; }
            set
            {
                if (Equals(value, hero))
                {
                    return;
                }
                hero = value;
                NotifyOfPropertyChange(() => Hero);
            }
        }

        public Hero OpponentHero
        {
            get { return opponentHero; }
            set
            {
                if (Equals(value, opponentHero))
                {
                    return;
                }
                opponentHero = value;
                NotifyOfPropertyChange(() => OpponentHero);
            }
        }

        public bool HasCoin
        {
            get { return hasCoin; }
            set
            {
                if (value.Equals(hasCoin))
                {
                    return;
                }
                hasCoin = value;
                NotifyOfPropertyChange(() => HasCoin);
            }
        }

        public GameMode GameMode
        {
            get { return gameMode; }
            set
            {
                if (value == gameMode)
                {
                    return;
                }
                gameMode = value;
                NotifyOfPropertyChange(() => GameMode);
            }
        }

        public void SetGameResult(GameStarted gameResult)
        {
            if (gameResult == null)
            {
                throw new ArgumentNullException("gameResult");
            }

            GameMode = gameResult.GameMode;
            Hero = heroRepository.Query(q => q.FirstOrDefault(x => x.Key == gameResult.Hero));
            OpponentHero = heroRepository.Query(q => q.FirstOrDefault(x => x.Key == gameResult.OpponentHero));
        }
    }
}
