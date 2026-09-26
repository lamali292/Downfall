using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.Powers;
using SlimeBoss.SlimeBossCode.Core;

namespace SlimeBoss.SlimeBossCode.Cards.Rare;

[Pool(typeof(SlimeBossCardPool))]
public class ShapeOfPuddle : SlimeBossCardModel
{
    public ShapeOfPuddle() : base(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        WithKeyword(CardKeyword.Exhaust);
        WithPower<IntangiblePower>(1);
        WithPower<NoBlockPower>(3, -1, false);
        WithTip(StaticHoverTip.Block);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<IntangiblePower>(ctx, this);
        await CommonActions.ApplySelf<NoBlockPower>(ctx, this);
    }
}
