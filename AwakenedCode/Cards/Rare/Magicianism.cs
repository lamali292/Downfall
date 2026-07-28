using Awakened.AwakenedCode.Core;
using Awakened.AwakenedCode.Powers;
using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;

namespace Awakened.AwakenedCode.Cards.Rare;

[Pool(typeof(AwakenedCardPool))]
public class Magicianism : AwakenedCardModel
{
    public Magicianism() : base(0, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
        WithTip(StaticHoverTip.Block);
    }
    protected override bool HasEnergyCostX => true;
    protected override Artist Artist => Artist.Get<Chimedragon>();

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var num1 = ResolveEnergyXValue();
        if (IsUpgraded) num1 ++;
        await CommonActions.ApplySelf<MagicianismPower>(ctx, this, num1);
    }
}