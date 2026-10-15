using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using SlimeBoss.SlimeBossCode.Core;

namespace SlimeBoss.SlimeBossCode.Cards.Common;

[Pool(typeof(SlimeBossCardPool))]
public class Vanish : SlimeBossCardModel
{
    public Vanish() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        WithKeyword(CardKeyword.Retain, UpgradeType.Add);
        WithKeyword(CardKeyword.Exhaust);
        WithBlock(5);
        WithCalculatedVar("Repeat", 1, Calc);
    }

    private static decimal Calc(CardModel card, Creature? _)
    {
        return card.Owner.Hand.Count(e => e.Type == CardType.Status);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var times = (int)((CalculatedVar)DynamicVars["Repeat"]).Calculate(null);
        for (var i = 0; i < times; i++)
            await CommonActions.CardBlock(this, cardPlay);
    }
}
