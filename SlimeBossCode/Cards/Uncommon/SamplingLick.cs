using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using SlimeBoss.SlimeBossCode.Core;
using SlimeBoss.SlimeBossCode.CustomEnums;
using SlimeBoss.SlimeBossCode.Powers;

namespace SlimeBoss.SlimeBossCode.Cards.Uncommon;

[Pool(typeof(SlimeBossCardPool))]
public class SamplingLick : SlimeBossCardModel
{
    public SamplingLick() : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        WithTags(SlimeBossTag.Lick);
        WithBlock(4);
        WithPower<GoopPower>(4);
        WithKeywords(CardKeyword.Exhaust);
        WithCards(0, 1);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.Apply<GoopPower>(ctx, this, cardPlay);
        await CommonActions.CardBlock(this, cardPlay);
        await CommonActions.Draw(this, ctx);
    }
}