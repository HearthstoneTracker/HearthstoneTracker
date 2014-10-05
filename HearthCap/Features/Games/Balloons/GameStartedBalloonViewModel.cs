// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GameStartedBalloonViewModel.cs" company="">
//   
// </copyright>
// <summary>
//   The game started balloon view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Features.Games.Balloons
{
    using System;
    using System.ComponentModel.Composition;
    using System.Linq;

    using Caliburn.Micro;

    using HearthCap.Core.GameCapture.HS.Events;
    using HearthCap.Data;
    using HearthCap.Features.GameManager;

    /// <summary>
    /// The game started balloon view model.
    /// </summary>
    [Export(typeof(GameStartedBalloonViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class GameStartedBalloonViewModel : PropertyChangedBase
    {
        /// <summary>
        /// The events.
        /// </summary>
        private readonly IEventAggregator events;

        /// <summary>
        /// The game manager.
        /// </summary>
        private readonly GameManager gameManager;

        /// <summary>
        /// The hero repository.
        /// </summary>
        private readonly IRepository<Hero> heroRepository;

        /// <summary>
        /// The has coin.
        /// </summary>
        private bool hasCoin;

        /// <summary>
        /// The opponent hero.
        /// </summary>
        private Hero opponentHero;

        /// <summary>
        /// The hero.
        /// </summary>
        private Hero hero;

        /// <summary>
        /// The game mode.
        /// </summary>
        private GameMode gameMode;

        /// <summary>
        /// Initializes a new instance of the <see cref="GameStartedBalloonViewModel"/> class.
        /// </summary>
        /// <param name="events">
        /// The events.
        /// </param>
        /// <param name="gameManager">
        /// The game manager.
        /// </param>
        /// <param name="heroRepository">
        /// The hero repository.
        /// </param>
        [ImportingConstructor]
        public GameStartedBalloonViewModel(IEventAggregator events, GameManager gameManager, IRepository<Hero> heroRepository)
        {
            this.events = events;
            this.gameManager = gameManager;
            this.heroRepository = heroRepository;
        }

        /// <summary>
        /// Gets or sets the hero.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the opponent hero.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating whether has coin.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the game mode.
        /// </summary>
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

        /// <summary>
        /// The set game result.
        /// </summary>
        /// <param name="gameResult">
        /// The game result.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// </exception>
        public void SetGameResult(GameStarted gameResult)
        {
            if (gameResult == null)
            {
                throw new ArgumentNullException("gameResult");
            }

            this.GameMode = gameResult.GameMode;
            this.Hero = this.heroRepository.Query(q => q.FirstOrDefault(x => x.Key == gameResult.Hero));
            this.OpponentHero = this.heroRepository.Query(q => q.FirstOrDefault(x => x.Key == gameResult.OpponentHero));
        }
    }
}
