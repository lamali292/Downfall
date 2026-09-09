using Collector.CollectorCode.Core;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Afflictions;

namespace Collector.CollectorCode.Powers;

public class KnightsCardPower : CollectorPowerModel
{
    public override bool TryModifyKeywordsInCombat(CardModel card, ISet<CardKeyword> keywords)
    {
        return card.Owner == Owner.Player && keywords.Add(CardKeyword.Ethereal);
    }
}