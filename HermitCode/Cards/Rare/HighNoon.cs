using Hermit.HermitCode.CustomEnums;
using Hermit.HermitCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Hermit.HermitCode.Cards.Rare;

/// <summary>
///     Whenever you play a Strike or Defend, draw a card.
///     Upgrade: Cost reduced from 2 to 1.
/// </summary>
public sealed class HighNoon : HermitCardModel
{
    public HighNoon() : base(1, CardType.Power, CardRarity.Rare, TargetType.None)
    {
        WithCostUpgradeBy(-1);
        WithTip(HermitKeywords.Strike);
        WithTip(HermitKeywords.Defend);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await PowerCmd.Apply<HighNoonPower>(ctx, Owner.Creature, 1, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.FinalizeUpgrade();
    }
}

/* transform_cards.py changes:
 *   namespace → Hermit.HermitCode.Cards.Rare
 *   usings updated
 *   OnUpgrade: migrated lines stripped, remainder kept
 *   constructor: WithCostUpgradeBy(-1)
 */