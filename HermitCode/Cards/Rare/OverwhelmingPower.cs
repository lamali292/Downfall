using Hermit.HermitCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Hermit.HermitCode.Cards.Rare;

/// <summary>
///     Gain 3 Energy. Draw 3 cards. Whenever you end your turn with 0 Energy, lose 4 HP.
///     Upgrade: Draw 4 cards. Lose 3 HP instead.
/// </summary>
public sealed class OverwhelmingPower : HermitCardModel
{
    public OverwhelmingPower() : base(1, CardType.Power, CardRarity.Rare, TargetType.None)
    {
        WithCards(3, 1);
        WithEnergy(3);
        WithHpLoss(4, -1);
    }


    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await PlayerCmd.GainEnergy(DynamicVars.Energy.IntValue, Owner);
        await CardPileCmd.Draw(ctx, DynamicVars.Cards.IntValue, Owner);
        await PowerCmd.Apply<OverwhelmingPowerPower>(ctx, Owner.Creature, DynamicVars["HpLoss"].IntValue,
            Owner.Creature, this);
    }
}

/* transform_cards.py changes:
 *   namespace → Hermit.HermitCode.Cards.Rare
 *   usings updated
 *   OnUpgrade removed (all logic migrated to constructor)
 *   constructor: WithCards(3, 1)
 */