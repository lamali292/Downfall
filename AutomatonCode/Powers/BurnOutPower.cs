using Automaton.AutomatonCode.Core;
using Automaton.AutomatonCode.Piles;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Automaton.AutomatonCode.Powers;

public class BurnOutPower : AutomatonPowerModel
{
    protected override async Task AfterCardChangedPiles(PlayerChoiceContext ctx, CardModel card, PileType oldPileType,
        AbstractModel? clonedBy)
    {
        if (card.Owner.Creature == Owner && card.Type is CardType.Status or CardType.Curse &&
            card.Pile?.Type == StashPile.Stash)
        {
            await CardCmd.Exhaust(ctx, card);
            var enemies = card.Owner.Creature.CombatState?.HittableEnemies;
            if (enemies == null) return;
            Flash();
            await CreatureCmd.Damage(ctx, enemies, Amount, ValueProp.Unpowered, Owner);
        }
    }
}