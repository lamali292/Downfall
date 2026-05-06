using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using Snecko.SneckoCode.Core;
using Snecko.SneckoCode.Events;

namespace Snecko.SneckoCode.Powers;

public class TsunamiPower : SneckoPowerModel, IAfterOverflowEffect
{
    public async Task AfterOverflowEffect(PlayerChoiceContext ctx, CardPlay cardPlay, CardModel card)
    {
        if (card.Owner.Creature != Owner) return;
        await CreatureCmd.GainBlock(Owner, Amount, ValueProp.Move | ValueProp.Unpowered, null);
        Flash();
    }
}