using BaseLib.Utils;
using Hermit.HermitCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Hermit.HermitCode.Cards.Uncommon;

public sealed class Gestalt : HermitCardModel
{
    public Gestalt() : base(2, CardType.Skill, CardRarity.Uncommon, TargetType.None)
    {
        WithPower<RuggedPower>(2);
        WithPower<VulnerablePower>(2, -1);
        WithKeyword(CardKeyword.Exhaust);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await CommonActions.ApplySelf<RuggedPower>(ctx, this);
        await CommonActions.ApplySelf<VulnerablePower>(ctx, this);
    }
}