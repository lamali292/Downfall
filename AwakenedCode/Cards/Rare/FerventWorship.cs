using Awakened.AwakenedCode.Cards.Token;
using Awakened.AwakenedCode.Core;
using Awakened.AwakenedCode.Powers;
using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;

namespace Awakened.AwakenedCode.Cards.Rare;

[Pool(typeof(AwakenedCardPool))]
public class FerventWorship : AwakenedCardModel
{
    public FerventWorship() : base(1, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
        WithEnergyTip();
        this.WithTip<Ceremony>();
        WithTip(StaticHoverTip.ReplayStatic);
        WithCostUpgradeBy(-1);
    }

    protected override Artist Artist => Artist.Get<Chimedragon>();

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<FerventWorshipPower>(ctx, this, 1);
    }
}