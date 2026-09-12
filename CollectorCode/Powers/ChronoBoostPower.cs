using Collector.CollectorCode.Core;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Collector.CollectorCode.Powers;

public class ChronoBoostPower : CollectorPowerModel
{
    private CardModel? _appliedCard;

    public ChronoBoostPower()
    {
        WithVar("CardPlays", 12);
    }

    public override Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        _appliedCard = cardSource;
        return Task.CompletedTask;
    }

    protected override int? SecondAmount => CardsPlayed;
    private int CardsPlayed { get; set; }

    public override async Task AfterCardPlayed(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (_appliedCard == cardPlay.Card)
        {
            _appliedCard = null;
            return;
        }
        if (cardPlay.Card.Owner.Creature != Owner) return;
        CardsPlayed++;
        InvokeDisplayAmountChanged();
        if (CardsPlayed < DynamicVars["CardPlays"].IntValue) return;
        await PowerCmd.Apply<StrengthPower>(ctx, Owner, Amount, Owner, null);
        await Cmd.CustomScaledWait(0.2f, 0.5f);
        CardsPlayed = 0;
        InvokeDisplayAmountChanged();
    }
}