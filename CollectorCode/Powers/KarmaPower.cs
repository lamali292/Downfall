using Collector.CollectorCode.Cards.Token;
using Collector.CollectorCode.Core;
using Downfall.DownfallCode.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Collector.CollectorCode.Powers;

/// <summary>Kindled Ash: the first card you Exhaust each turn leaves an Ember in your Discard Pile.</summary>
public class KarmaPower : CollectorPowerModel
{
    private bool _usedThisTurn;

    public KarmaPower()
    {
        WithTip<Ember>();
    }

    public override async Task AfterCardExhausted(PlayerChoiceContext ctx, CardModel card, bool causedByEthereal)
    {
        // Embers are excluded so exhausting the Ember it just made can't chain.
        if (_usedThisTurn || card is Ember || card.Owner.Creature != Owner) return;
        _usedThisTurn = true;
        Flash();
        await DownfallCardCmd.GiveCards<Ember>(card.Owner, PileType.Discard, Amount);
    }

    public override Task BeforeSideTurnEnd(PlayerChoiceContext ctx, CombatSide side, IEnumerable<Creature> participants)
    {
        if (side == Owner.Side) _usedThisTurn = false;
        return Task.CompletedTask;
    }
}
