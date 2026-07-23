using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using Snecko.SneckoCode.Core;
using Snecko.SneckoCode.CustomEnums;
using Snecko.SneckoCode.Events;
using Snecko.SneckoCode.History;

namespace Snecko.SneckoCode.Powers;

public class WeightedDicePower() : SneckoPowerModel(PowerType.Buff, PowerStackType.Single)
{
    public override int ModifyCardPlayCount(CardModel card, Creature? target, int playCount)
    {
        if (card.Owner.Creature != Owner || card.EnergyCost.CostsX || card.EnergyCost.GetResolved() <= card.EnergyCost.GetWithModifiers(default)) return playCount;

        return playCount + 1;
    }

    public override Task AfterModifyingCardPlayCount(CardModel card)
    {
        Flash();
        return Task.CompletedTask;
    }

}