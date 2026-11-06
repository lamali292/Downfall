using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using Downfall.DownfallCode.Powers;
using Hexaghost.HexaghostCode.Core;
using Hexaghost.HexaghostCode.CustomEnums;
using Hexaghost.HexaghostCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Hexaghost.HexaghostCode.Cards.Rare;

[Pool(typeof(HexaghostCardPool))]
public class VolcanoVisage : HexaghostCardModel
{
    public VolcanoVisage() : base(1, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
        WithPower<VolcanoVisagePower>(4, 2, false);
        WithTip(HexaghostTip.Ignite);
        WithTip<SoulBurnPower>();
    }

    protected override Artist Artist => Artist.Get<CartesianCanvas>();

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<VolcanoVisagePower>(ctx, this);
    }
}