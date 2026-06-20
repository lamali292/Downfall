using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using SlimeBoss.SlimeBossCode.Core;
using SlimeBoss.SlimeBossCode.CustomEnums;
using SlimeBoss.SlimeBossCode.Powers;

namespace SlimeBoss.SlimeBossCode.Cards.Common;

[Pool(typeof(SlimeBossCardPool))]
public class DoubleLick : SlimeBossCardModel
{
    public DoubleLick() : base(0, CardType.Skill, CardRarity.Common, TargetType.AnyEnemy)
    {
        WithPower<GoopPower>(4);
        this.WithRepeat(2);
        WithKeywords(CardKeyword.Exhaust);
        WithCards(0, 1);
        WithTags(SlimeBossTag.Lick);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var repeat = DynamicVars.Repeat.BaseValue;
        for (var i = 0; i < repeat; i++)
            await CommonActions.Apply<GoopPower>(ctx, this, cardPlay);
        await CommonActions.Draw(this, ctx);
    }
}