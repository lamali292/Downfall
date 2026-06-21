using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using Hexaghost.HexaghostCode.Core;
using Hexaghost.HexaghostCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;

namespace Hexaghost.HexaghostCode.Cards.Rare;

[Pool(typeof(HexaghostCardPool))]
public class RadiantFlame : HexaghostCardModel
{
    public RadiantFlame() : base(1, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
        WithPower<RadiantFlamePower>(2, 1);
        WithTip(StaticHoverTip.Block);
    }

    protected override Artist Artist => Artist.Get<GoofballMcgee>();

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<RadiantFlamePower>(ctx, this);
    }
}