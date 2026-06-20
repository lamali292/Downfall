using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.CardPools;
using SlimeBoss.SlimeBossCode.Core;
using SlimeBoss.SlimeBossCode.CustomEnums;
using SlimeBoss.SlimeBossCode.Powers;

namespace SlimeBoss.SlimeBossCode.Cards.Token;

[Pool(typeof(TokenCardPool))]
public class Lick : SlimeBossCardModel
{
    public Lick() : base(0, CardType.Skill, CardRarity.Token, TargetType.AnyEnemy)
    {
        WithPower<GoopPower>(4, 2);
        WithKeywords(CardKeyword.Exhaust, SlimeBossKeyword.Buried);
        WithTip(SlimeBossKeyword.Slurp);
        WithTags(SlimeBossTag.Lick);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.Apply<GoopPower>(ctx, this, cardPlay);
    }
}