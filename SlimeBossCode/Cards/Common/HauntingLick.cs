using BaseLib.Utils;
using Downfall.DownfallCode.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using SlimeBoss.SlimeBossCode.Core;
using SlimeBoss.SlimeBossCode.CustomEnums;
using SlimeBoss.SlimeBossCode.Powers;

namespace SlimeBoss.SlimeBossCode.Cards.Common;

[Pool(typeof(SlimeBossCardPool))]
public class HauntingLick : SlimeBossCardModel
{
    public HauntingLick() : base(0, CardType.Skill, CardRarity.Common, TargetType.AnyEnemy)
    {
        WithTags(SlimeBossTag.Lick);
        WithPower<VulnerablePower>(1);
        WithPower<GoopPower>(4);
        WithKeywords(CardKeyword.Exhaust);
        WithCards(0, 1);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await MyCommonActions.Apply<VulnerablePower>(ctx, this, cardPlay);
        await MyCommonActions.Apply<GoopPower>(ctx, this, cardPlay);
        await CommonActions.Draw(this, ctx);
    }
}