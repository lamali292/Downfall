using Downfall.DownfallCode.Commands;
using Hermit.HermitCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Hermit.HermitCode.Cards.Uncommon;

public sealed class Horror : HermitCardModel
{
    public Horror() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.AllEnemies)
    {
        WithPower<BruisePower>(3, 2);
        WithPower<HorrorPower>(1, false);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await MyCommonActions.Apply<BruisePower>(ctx, this, play);
        await MyCommonActions.Apply<HorrorPower>(ctx, this, play);
    }
}