using Downfall.DownfallCode.Artists;
using Downfall.DownfallCode.Commands;
using Hermit.HermitCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Hermit.HermitCode.Cards.Rare;

public class Burdened : HermitCardModel
{
    public Burdened() : base(1, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
        WithPower<BurdenedPower>(8, 3, false);
        WithTip<VigorPower>();
        WithTip<Decay>();
    }
    
    protected override Artist Artist => Artist.Get<Fulgur>();
    protected override bool IsBetaArt => true;

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        (await MyCommonActions.ApplySelf<BurdenedPower>(ctx, this))?.IncrementSelfDamage();
    }
}