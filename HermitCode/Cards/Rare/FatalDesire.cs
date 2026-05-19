using BaseLib.Utils;
using Hermit.HermitCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Hermit.HermitCode.Cards.Rare;

public sealed class FatalDesire : HermitCardModel
{
    public FatalDesire() : base(1, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
        WithPower<FatalDesirePower>(1);
        WithPower<MachineLearningPower>(2);
        WithKeyword(CardKeyword.Innate, UpgradeType.Add);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await CommonActions.ApplySelf<MachineLearningPower>(ctx, this);
        await CommonActions.ApplySelf<FatalDesirePower>(ctx, this);
    }
}
