using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using SlimeBoss.SlimeBossCode.Core;
using SlimeBoss.SlimeBossCode.Extensions;

namespace SlimeBoss.SlimeBossCode.Cards.Rare;

[Pool(typeof(SlimeBossCardPool))]
public class SlimeTap : SlimeBossCardModel
{
    public SlimeTap() : base(2, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        WithCostUpgradeBy(-1);
        WithKeyword(CardKeyword.Exhaust);
        WithCalculatedVar("Slimes", 0, Calc);
    }

    private static decimal Calc(CardModel card, Creature? _)
    {
        return card.Owner.SlimeCount;
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var a = DynamicVars["Slimes"].Calculate(cardPlay.Target);
        await PlayerCmd.GainEnergy(a, Owner);
        await CardPileCmd.Draw(ctx, a, Owner);
    }
}
