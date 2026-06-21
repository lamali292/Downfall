using BaseLib.Utils;
using Hexaghost.HexaghostCode.Core;
using Hexaghost.HexaghostCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;

namespace Hexaghost.HexaghostCode.Cards.Uncommon;

[Pool(typeof(HexaghostCardPool))]
public class WildfireHexaghost : HexaghostCardModel
{
    public WildfireHexaghost() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
    {
        this.WithPower<WildfirePower>(5, 2, false);
        WithTip(StaticHoverTip.Block);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<WildfirePower>(ctx, this);
    }
}