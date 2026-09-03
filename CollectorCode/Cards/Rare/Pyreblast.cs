using BaseLib.Abstracts;
using BaseLib.Utils;
using Collector.CollectorCode.Core;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Collector.CollectorCode.Cards.Rare;

[Pool(typeof(CollectorCardPool))]
public class Pyreblast : CollectorCardModel
{
    public Pyreblast() : base(10, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
    {
        WithKeyword(CardKeyword.Retain);
        WithDamage(60, 40);
        WithEnergy(1);
        WithTip(CardKeyword.Exhaust);
    }

    public override Task AfterCardExhausted(PlayerChoiceContext ctx, CardModel card, bool causedByEthereal)
    {
        if (card.Owner != Owner || card.Type is not (CardType.Curse or CardType.Status) || Pile is not { Type: PileType.Hand }) return Task.CompletedTask;
        EnergyCost.AddThisCombat(-DynamicVars.Energy.IntValue);
        return Task.CompletedTask;
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
    }
}