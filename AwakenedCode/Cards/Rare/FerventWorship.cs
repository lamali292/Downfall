using Awakened.AwakenedCode.Core;
using Awakened.AwakenedCode.Powers;
using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Awakened.AwakenedCode.Cards.Rare;

[Pool(typeof(AwakenedCardPool))]
public class FerventWorship : AwakenedCardModel
{
    public FerventWorship() : base(1, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
        WithPower<CuriosityPower>(1);
        WithCostUpgradeBy(-1);
        WithPower<FerventWorshipPower>(1, false);
        WithTip<StrengthPower>();
    }

    protected override Artist Artist => Artist.Get<Chimedragon>();

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<CuriosityPower>(ctx, this);
        await CommonActions.ApplySelf<FerventWorshipPower>(ctx, this);
    }
}