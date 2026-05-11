using Downfall.DownfallCode.CustomEnums;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.DownfallCode.Extensions;

public static class CardModelExtensions
{
    public static CardModel CreateEcho(this CardModel card) => card.CreateClone().ToEcho();
    
    public static CardModel ToEcho(this CardModel card)
    {
        if (card.IsEcho())
            throw new InvalidOperationException($"Card {card.Id} is already an Echo.");
        card.AddKeyword(CardKeyword.Exhaust);
        card.AddKeyword(CardKeyword.Ethereal);
        card.AddKeyword(DownfallKeyword.Echo);
        return card;
    }
    
    public static bool IsEcho(this CardModel card)
    {
        return card.Keywords.Contains(DownfallKeyword.Echo);
    }
}