using BaseLib.Utils;
using Downfall.DownfallCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Hermit.HermitCode.Cards.Common;

public sealed class Dive : HermitCardModel, IHasDeadOnEffect
{
    public Dive() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        WithBlock(8, 2);
        WithPower<PlatedArmorPower>(1, 1);
    }

    public async Task DeadOnEffect(PlayerChoiceContext ctx, CardPlay play)
    {
        await CommonActions.ApplySelf<PlatedArmorPower>(ctx, this);
    }


    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await CommonActions.CardBlock(this, play);
    }
}