using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Hermit.HermitCode.Cards.Rare;

public class Burdened : HermitCardModel, IHasDeadOnEffect
{
    public Burdened() : base(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        WithBlock(10, 3);
        WithCards(3, 1);
    }

    protected override Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        return CommonActions.CardBlock(this, cardPlay);
    }

    public async Task DeadOnEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var cards = Owner.GetDraw().Where(e => e.Type == CardType.Curse)
            .TakeRandom(DynamicVars.Cards.IntValue, Owner.RunState.Rng.CombatCardSelection);
        await CardPileCmd.Add(cards, PileType.Hand);
    }
}