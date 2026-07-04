using BaseLib.Utils;
using Downfall.DownfallCode.Commands;
using Hermit.HermitCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Hermit.HermitCode.Cards.Rare;

public class Burdened : HermitCardModel
{
    public Burdened() : base(1, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
        this.WithPower<BurdenedPower>(8, 2, false);
        this.WithTip<VigorPower>();
        this.WithTip<Decay>();
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        (await MyCommonActions.ApplySelf<BurdenedPower>(ctx, this))?.IncrementSelfDamage();
    }
 
}