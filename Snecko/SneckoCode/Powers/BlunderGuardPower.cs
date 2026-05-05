using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;
using Snecko.SneckoCode.Core;

namespace Snecko.SneckoCode.Powers;

public class BlunderGuardPower : SneckoPowerModel
{
    public override async Task AfterCardPlayed(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (cardPlay.Resources.EnergySpent < 3 || cardPlay.Card.Owner.Creature != Owner) return;
        await CreatureCmd.GainBlock(Owner, Amount, ValueProp.Unpowered, null);
    }
}