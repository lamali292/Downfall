using BaseLib.Utils;
using Hermit.HermitCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Hermit.HermitCode.Cards.Multiplayer;

public class Technique : HermitCardModel
{
    public Technique() : base(1, CardType.Power, CardRarity.Rare, TargetType.AllAllies)
    {
        this.WithPower<MaintenanceStrikePower>(5, 2, false);
    }

    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;


    protected override Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        return CommonActions.Apply<MaintenanceStrikePower>(ctx, this, cardPlay);
    }
}