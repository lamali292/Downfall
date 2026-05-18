using Hermit.HermitCode.CustomEnums;
using Hermit.HermitCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Hermit.HermitCode.Cards.Uncommon;

public sealed class BigShot : HermitCardModel
{
    private const int BigShotAmount = 3;
    private const int UpgradedBigShotAmount = 4;

    public BigShot() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.None)
    {
        WithPower<BigShotPower>(3, 1);
        WithTip(typeof(VigorPower));
        WithTip(HermitKeywords.DeadOn);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await PowerCmd.Apply<BigShotPower>(ctx, Owner.Creature, DynamicVars["BigShotPower"].BaseValue, Owner.Creature,
            this);
    }
}

/* transform_cards.py changes:
 *   namespace → Hermit.HermitCode.Cards.Uncommon
 *   usings updated
 *   OnUpgrade removed (all logic migrated to constructor)
 *   constructor: WithPower<BigShotPower>(3, 1)
 */