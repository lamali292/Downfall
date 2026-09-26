using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using SlimeBoss.SlimeBossCode.Core;

namespace SlimeBoss.SlimeBossCode.Cards.Common;

[Pool(typeof(SlimeBossCardPool))]
public class FearTactics : SlimeBossCardModel
{
    public FearTactics() : base(1, CardType.Skill, CardRarity.Common, TargetType.AnyEnemy)
    {
        WithKeywords(CardKeyword.Retain, CardKeyword.Exhaust);
        WithPower<WeakPower>(2, 1);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.Apply<WeakPower>(ctx, this, cardPlay);
    }
}
