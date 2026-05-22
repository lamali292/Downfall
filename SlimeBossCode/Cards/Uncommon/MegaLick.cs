using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using SlimeBoss.SlimeBossCode.Core;
using SlimeBoss.SlimeBossCode.CustomEnums;
using SlimeBoss.SlimeBossCode.Powers;

namespace SlimeBoss.SlimeBossCode.Cards.Uncommon;

[Pool(typeof(SlimeBossCardPool))]
public class MegaLick : SlimeBossCardModel
{
    public MegaLick() : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.AllEnemies)
    {
        WithTags(SlimeBossTag.Lick);
        WithPower<WeakPower>(1);
        WithPower<GoopPower>(4);
        WithKeywords(CardKeyword.Exhaust);
        WithCards(0, 1);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.Apply<WeakPower>(ctx, this, cardPlay);
        await CommonActions.Apply<VulnerablePower>(ctx, this, cardPlay);
        await CommonActions.Draw(this, ctx);
    }
}