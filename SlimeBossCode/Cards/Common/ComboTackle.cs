using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using SlimeBoss.SlimeBossCode.Core;
using SlimeBoss.SlimeBossCode.CustomEnums;
using SlimeBoss.SlimeBossCode.Powers;

namespace SlimeBoss.SlimeBossCode.Cards.Common;

[Pool(typeof(SlimeBossCardPool))]
public class ComboTackle : SlimeBossCardModel
{
    public ComboTackle() : base(2, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
        WithTags(SlimeBossTag.Tackle);
        WithDamage(10, 2);
        WithPower<ComboTackleDiscountPower>(1, false);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
        // TODO - remove "this turn" on upgrade
        await CommonActions.ApplySelf<ComboTackleDiscountPower>(ctx, this);
    }
}
