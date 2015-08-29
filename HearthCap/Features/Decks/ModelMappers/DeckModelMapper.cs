using System.Collections.Generic;
using System.Linq;
using HearthCap.Data;
using Omu.ValueInjecter;

namespace HearthCap.Features.Decks.ModelMappers
{
    public static class DeckModelMapper
    {
        public static DeckModel ToModel(this Deck deck)
        {
            if (deck == null)
            {
                return null;
            }
            var model = new DeckModel();
            model.InjectFrom(deck);
            return model;
        }

        public static IEnumerable<DeckModel> ToModel(this IEnumerable<Deck> decks)
        {
            return decks.Where(x => x != null).Select(deck => deck.ToModel()).ToList();
        }
    }
}
