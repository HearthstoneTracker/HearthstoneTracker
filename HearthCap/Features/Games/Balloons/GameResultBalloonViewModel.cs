// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GameResultBalloonViewModel.cs" company="">
//   
// </copyright>
// <summary>
//   The game result balloon view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Features.Games.Balloons
{
    using System;
    using System.ComponentModel.Composition;

    using Caliburn.Micro;

    using HearthCap.Data;
    using HearthCap.Features.GameManager;
    using HearthCap.Features.Games.Models;

    /// <summary>
    /// The game result balloon view model.
    /// </summary>
    [Export(typeof(GameResultBalloonViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class GameResultBalloonViewModel : PropertyChangedBase
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
        /// The victory.
        /// </summary>
        private bool victory;

        /// <summary>
        /// The opponent hero.
        /// </summary>
        private Hero opponentHero;

        /// <summary>
        /// The hero.
        /// </summary>
        private Hero hero;

        /// <summary>
        /// The game result.
        /// </summary>
        private GameResultModel gameResult;

        /// <summary>
        /// The game mode.
        /// </summary>
        private GameMode gameMode;

        /// <summary>
        /// The turns.
        /// </summary>
        private int turns;

        /// <summary>
        /// Initializes a new instance of the <see cref="GameResultBalloonViewModel"/> class.
        /// </summary>
        /// <param name="events">
        /// The events.
        /// </param>
        /// <param name="gameManager">
        /// The game manager.
        /// </param>
        [ImportingConstructor]
        public GameResultBalloonViewModel(IEventAggregator events, GameManager gameManager)
        {
            this.events = events;
            this.gameManager = gameManager;
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
        /// Gets or sets a value indicating whether victory.
        /// </summary>
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
        /// Gets or sets the turns.
        /// </summary>
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

        /// <summary>
        /// The correct win.
        /// </summary>
        public async void CorrectWin()
        {
            // events.PublishOnBackgroundThread(new CorrectLastGameResult(gameResult.Id){ Won = true });
            if (this.gameResult == null) return;

            this.gameResult.Victory = true;
            this.Victory = true;
            await this.gameManager.UpdateGame(this.gameResult);
        }

        /// <summary>
        /// The set game result.
        /// </summary>
        /// <param name="gameResult">
        /// The game result.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// </exception>
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
