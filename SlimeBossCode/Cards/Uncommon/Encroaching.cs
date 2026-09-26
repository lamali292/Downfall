using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using SlimeBoss.SlimeBossCode.Core;
using SlimeBoss.SlimeBossCode.Powers;

namespace SlimeBoss.SlimeBossCode.Cards.Uncommon;

[Pool(typeof(SlimeBossCardPool))]
public class Encroaching : SlimeBossCardModel
{
    public Encroaching() : base(2, CardType.Power, CardRarity.Uncommon, TargetType.Self)
    {
        WithPower<EncroachingPower>(1, false);
        WithCostUpgradeBy(-1);
        WithTip<WeakPower>();

    }

    protected override Artist Artist => Artist.Get<Thelethargicweirdo>();

    protected override Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        return CommonActions.ApplySelf<EncroachingPower>(ctx, this);
    }
}
