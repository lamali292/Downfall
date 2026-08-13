using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using Hexaghost.HexaghostCode.Core;
using Hexaghost.HexaghostCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Hexaghost.HexaghostCode.Cards.Uncommon;

[Pool(typeof(HexaghostCardPool))]
public class EmpoweredFlame : HexaghostCardModel
{
    public EmpoweredFlame() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
    {
        WithPower<IntensityPower>(1, 1);
    }

    protected override Artist Artist => Artist.Get<Claude27A>();

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<IntensityPower>(ctx, this);
    }
}