using Awakened.AwakenedCode.Core;
using Awakened.AwakenedCode.Powers;
using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Void = MegaCrit.Sts2.Core.Models.Cards.Void;

namespace Awakened.AwakenedCode.Cards.Rare;

[Pool(typeof(AwakenedCardPool))]
public class EclipseEmbrace : AwakenedCardModel
{
    public EclipseEmbrace() : base(2, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
        WithTip(CardKeyword.Exhaust);
        this.WithTip<Void>();
        this.WithPower<EclipseEmbracePower>(1, false);
        WithEnergyTip();
        WithCostUpgradeBy(-1);
    }

    protected override Artist Artist => Artist.Get<Opal>();


    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<EclipseEmbracePower>(ctx, this);
    }
}