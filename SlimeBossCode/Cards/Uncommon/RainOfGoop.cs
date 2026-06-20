using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using SlimeBoss.SlimeBossCode.Core;
using SlimeBoss.SlimeBossCode.Powers;

namespace SlimeBoss.SlimeBossCode.Cards.Uncommon;

[Pool(typeof(SlimeBossCardPool))]
public class RainOfGoop : SlimeBossCardModel
{
    public RainOfGoop() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.RandomEnemy)
    {
        WithPower<GoopPower>(4);
        this.WithRepeat(3, 1);
    }


    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var repeat = DynamicVars.Repeat.BaseValue;
        for (var i = 0; i < repeat; i++)
            await CommonActions.Apply<GoopPower>(ctx, this, cardPlay);
    }
}