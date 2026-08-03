using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using Hexaghost.HexaghostCode.Core;
using Hexaghost.HexaghostCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;

namespace Hexaghost.HexaghostCode.Cards.Multiplayer;

[Pool(typeof(HexaghostCardPool))]
public class Heating : HexaghostCardModel
{
    public Heating() : base(2, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
        this.WithPower<HeatingPower>(1, 1, false);
        WithTip(StaticHoverTip.Block);
    }
    
    protected override Artist Artist => Artist.Get<Chimedragon>();

    
    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;
    
    protected override Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        return CommonActions.ApplySelf<HeatingPower>(ctx, this);
    }
}