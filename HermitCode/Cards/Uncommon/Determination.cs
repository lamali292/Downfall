using BaseLib.Utils;
using Hermit.HermitCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Hermit.HermitCode.Cards.Uncommon;

public sealed class Determination : HermitCardModel
{
    public Determination() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.None)
    {
        WithKeyword(CardKeyword.Innate, UpgradeType.Add);
        WithPower<DeterminationPower>(1, false);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await CommonActions.ApplySelf<DeterminationPower>(ctx, this);
    }
}