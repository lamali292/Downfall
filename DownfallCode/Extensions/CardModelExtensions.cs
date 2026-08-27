using Downfall.DownfallCode.CustomEnums;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.DownfallCode.Extensions;

public static class CardModelExtensions
{
    extension(CardModel card)
    {
        public bool IsEcho => card.Keywords.Contains(DownfallKeyword.Echo);

        public CardModel CreateEcho()
        {
            return card.CreateClone().ToEcho();
        }

        public CardModel ToEcho()
        {
            if (card.IsEcho)
                throw new InvalidOperationException($"Card {card.Id} is already an Echo.");
            card.AddKeyword(CardKeyword.Exhaust);
            card.AddKeyword(CardKeyword.Ethereal);
            card.AddKeyword(DownfallKeyword.Echo);
            return card;
        }
    }
}