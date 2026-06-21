using BaseLib.Utils;
using Downfall.DownfallCode.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using SlimeBoss.SlimeBossCode.Core;
using SlimeBoss.SlimeBossCode.CustomEnums;

namespace SlimeBoss.SlimeBossCode.Cards.Common;

[Pool(typeof(SlimeBossCardPool))]
public class SpearTackle : SlimeBossCardModel
{
    public SpearTackle() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
        WithDamage(11, 2);
        this.WithSelfDamage(3);
        WithTags(SlimeBossTag.Tackle);
        WithCards(2, 1);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
        await MyCommonActions.SelfDamage(ctx, this);
        await CommonActions.Draw(this, ctx);
    }
}