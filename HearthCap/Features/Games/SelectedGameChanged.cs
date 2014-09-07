namespace HearthCap.Features.Games
{
    using System;

    using HearthCap.Features.Games.Models;

    public class SelectedGameChanged
    {
        public object Source { get; protected set; }

        public GameResultModel Game { get; protected set; }

        public SelectedGameChanged(object source, GameResultModel game)
        {
            this.Source = source;
            this.Game = game;
        }
    }
}