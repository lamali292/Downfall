using BaseLib.Utils;
using Hermit.HermitCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Hermit.HermitCode.Cards.Multiplayer;

public class BigBruiser : HermitCardModel
{
    public BigBruiser() : base(3, CardType.Power, CardRarity.Rare, TargetType.AllEnemies)
    {
        this.WithPower<BigBruiserPower>(1, false);
        WithPower<BruisePower>(3, 3);
    }
    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;


    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await CommonActions.ApplySelf<BigBruiserPower>(ctx, this);
        await CommonActions.Apply<BruisePower>(ctx, this, cardPlay);
    }
}