using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using SlimeBoss.SlimeBossCode.Core;

namespace SlimeBoss.SlimeBossCode.Cards.Common;

[Pool(typeof(SlimeBossCardPool))]
public class SlimeSpikes : SlimeBossCardModel
{
    public SlimeSpikes() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        WithBlock(7, 2);
        WithPower<SlimeSpikesPower>(2, 2, false);
        WithTip<ThornsPower>();
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardBlock(this, cardPlay);
        await CommonActions.ApplySelf<SlimeSpikesPower>(ctx, this);
    }

    protected override bool ShouldGlowGoldInternal => HasPlayedStatusThisTurn;

    private bool HasPlayedStatusThisTurn => CombatManager.Instance.History.Entries
        .OfType<CardPlayStartedEntry>()
        .Any(e => e.CardPlay.Card.Type == CardType.Status && e.CardPlay.Card.Owner == Owner &&
                  e.HappenedThisTurn(CombatState));

    public override Task AfterCardEnteredCombat(CardModel card)
    {
        if (card == this && HasPlayedStatusThisTurn) EnergyCost.SetThisTurn(0);
        return Task.CompletedTask;
    }

    public override Task AfterCardPlayed(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (cardPlay.Card.Type == CardType.Status && cardPlay.Card.Owner == Owner) EnergyCost.SetThisTurn(0);
        return Task.CompletedTask;
    }
}

public class SlimeSpikesPower : CustomTemporaryPowerModelWrapper<SlimeSpikes, ThornsPower>
{
    protected override bool UntilEndOfOtherSideTurn => true;
}
