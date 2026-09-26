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
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
        if (IsUpgraded)
        {
            await CommonActions.ApplySelf<ComboTacklePlusPower>(ctx, this, 1);
        }
        else
        {
            await CommonActions.ApplySelf<ComboTacklePower>(ctx, this, 1);
        }
    }
}
