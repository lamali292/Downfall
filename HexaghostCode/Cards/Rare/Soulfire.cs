using BaseLib.Utils;
using Downfall.DownfallCode.Powers;
using Hexaghost.HexaghostCode.Core;
using Hexaghost.HexaghostCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;

namespace Hexaghost.HexaghostCode.Cards.Rare;

[Pool(typeof(HexaghostCardPool))]
public class Soulfire : HexaghostCardModel
{
    public Soulfire() : base(2, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
        WithPower<SoulfirePower>(3, 1, false);
        WithTip(StaticHoverTip.Block);
        WithTip<SoulBurnPower>();
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<SoulfirePower>(ctx, this);
    }
}