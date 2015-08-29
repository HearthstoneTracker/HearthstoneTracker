using System;
using System.Linq;
using Caliburn.Micro;
using HearthCap.Data;

namespace HearthCap.Features.Core
{
    public class GameModesStringCollection : BindableCollection<string>
    {
        public GameModesStringCollection(bool emptyItem = false)
        {
            if (emptyItem)
            {
                Add("");
            }
            var values = Enum.GetValues(typeof(GameMode)).Cast<GameMode>().Select(x => x.ToString());
            AddRange(values);
        }
    }
}
