using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Cards.CardModels;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.Code.Cards.Awakened.Rare;

[Pool(typeof(AwakenedCardPool))]
public class Skyward : AwakenedCardModel
{
    public Skyward() : base(7, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        WithBlock(18);
        WithCards(1);
    }

    private int PowersPlayedThisCombat => CombatManager.Instance.History.Entries
        .OfType<CardPlayStartedEntry>()
        .Count(e =>
            e.CardPlay.Card.Type == CardType.Power &&
            e.CardPlay.Card.Owner == Owner);

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardBlock(this, cardPlay);
        await CommonActions.Draw(this, ctx);
    }


    public override Task AfterCardEnteredCombat(CardModel card)
    {
        if (card != this || IsClone)
            return Task.CompletedTask;
        ReduceCostBy(PowersPlayedThisCombat);
        return Task.CompletedTask;
    }

    public override Task BeforeCardPlayed(CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner != Owner || cardPlay.Card.Type != CardType.Power)
            return Task.CompletedTask;
        ReduceCostBy(1);
        return Task.CompletedTask;
    }

    private void ReduceCostBy(int amount)
    {
        EnergyCost.AddThisCombat(-amount);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(6);
    }
}