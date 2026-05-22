using BaseLib.Utils;
using Hermit.HermitCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Hermit.HermitCode.Cards.Common;

public class Feint : HermitCardModel
{
    public Feint() : base(0, CardType.Skill, CardRarity.Common, TargetType.AllEnemies)
    {
        WithBlock(3, 2);
        WithPower<BruisePower>(2, 1);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await CommonActions.CardBlock(this, play);
        await CommonActions.Apply<BruisePower>(ctx, this, play);
    }
}