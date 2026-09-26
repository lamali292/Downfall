using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using SlimeBoss.SlimeBossCode.Core;
using SlimeBoss.SlimeBossCode.Powers;

namespace SlimeBoss.SlimeBossCode.Cards.Rare;

[Pool(typeof(SlimeBossCardPool))]
public class ShapeOfPuddle : SlimeBossCardModel
{
    public ShapeOfPuddle() : base(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        WithKeyword(CardKeyword.Exhaust);
        WithPower<IntangiblePower>(1, false);
        WithPower<NoBlockFromCardsPower>(3, -1, false);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<IntangiblePower>(ctx, this);
        await CommonActions.ApplySelf<NoBlockFromCardsPower>(ctx, this);
    }
}
