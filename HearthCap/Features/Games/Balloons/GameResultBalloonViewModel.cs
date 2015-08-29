using System;
using System.ComponentModel.Composition;
using Caliburn.Micro;
using HearthCap.Data;
using HearthCap.Features.Games.Models;

namespace HearthCap.Features.Games.Balloons
{
    [Export(typeof(GameResultBalloonViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class GameResultBalloonViewModel : PropertyChangedBase
    {
        private readonly IEventAggregator events;

        private readonly GameManager.GameManager gameManager;

        private bool victory;

        private Hero opponentHero;

        private Hero hero;

        private GameResultModel gameResult;

        private GameMode gameMode;

        private int turns;

        [ImportingConstructor]
        public GameResultBalloonViewModel(IEventAggregator events, GameManager.GameManager gameManager)
        {
            this.events = events;
            this.gameManager = gameManager;
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

        public bool Victory
        {
            get { return victory; }
            set
            {
                if (value.Equals(victory))
                {
                    return;
                }
                victory = value;
                NotifyOfPropertyChange(() => Victory);
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

        public int Turns
        {
            get { return turns; }
            set
            {
                if (value == turns)
                {
                    return;
                }
                turns = value;
                NotifyOfPropertyChange(() => Turns);
            }
        }

        public async void CorrectWin()
        {
            // events.PublishOnBackgroundThread(new CorrectLastGameResult(gameResult.Id){ Won = true });
            if (gameResult == null)
            {
                return;
            }

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
            Hero = gameResult.Hero;
            OpponentHero = gameResult.OpponentHero;
            Victory = gameResult.Victory;
            GameMode = gameResult.GameMode;
            Turns = gameResult.Turns;
        }
    }
}
