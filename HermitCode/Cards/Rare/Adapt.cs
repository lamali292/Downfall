using Hermit.HermitCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Hermit.HermitCode.Cards.Rare;

public sealed class Adapt : HermitCardModel
{
    private const int AdaptAmount = 8;

    public Adapt() : base(3, CardType.Power, CardRarity.Rare, TargetType.None)
    {
        WithPower<AdaptPower>(8);
        WithCostUpgradeBy(-1);
        WithTip(CardKeyword.Exhaust);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await PowerCmd.Apply<AdaptPower>(ctx, Owner.Creature, 1, Owner.Creature, this);
    }
}

/* transform_cards.py changes:
 *   namespace → Hermit.HermitCode.Cards.Rare
 *   usings updated
 *   OnUpgrade removed (all logic migrated to constructor)
 *   constructor: WithPower<AdaptPower>(8, 0), WithCostUpgradeBy(-1)
 */