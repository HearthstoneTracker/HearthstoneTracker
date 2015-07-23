namespace HearthCap.Features.Core
{
    using System;
    using System.Linq;

    using Caliburn.Micro;

    using HearthCap.Data;

    public class GameModesStringCollection : BindableCollection<string>
    {
        public GameModesStringCollection(bool emptyItem = false)
        {
            if (emptyItem)
            {
                this.Add("");
            }
            var values = Enum.GetValues(typeof(GameMode)).Cast<GameMode>().Select(x => x.ToString());
            this.AddRange(values);
        }
    }
}