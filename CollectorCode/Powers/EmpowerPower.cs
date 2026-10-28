using Collector.CollectorCode.Cards.Token;
using Collector.CollectorCode.Core;
using Downfall.DownfallCode.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;

namespace Collector.CollectorCode.Powers;

/// <summary>Ashen Alchemy: a Burn that gets Exhausted turns into an Ember.</summary>
public class EmpowerPower : CollectorPowerModel
{
    public EmpowerPower()
    {
        WithTip<Burn>();
        WithTip<Ember>();
    }

    public override async Task AfterCardExhausted(PlayerChoiceContext ctx, CardModel card, bool causedByEthereal)
    {
        if (card is not Burn || card.Owner.Creature != Owner) return;
        Flash();
        await DownfallCardCmd.GiveCards<Ember>(card.Owner, PileType.Hand, Amount);
    }
}
