using Automaton.AutomatonCode.Core;
using Automaton.AutomatonCode.Events;
using Automaton.AutomatonCode.Piles;
using Downfall.DownfallCode.Compatibility;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Automaton.AutomatonCode.Powers;

public class BurnOutPower : AutomatonPowerModel, IAfterCardStashed
{
    public async Task AfterCardsStashed(PlayerChoiceContext ctx, Player player, IEnumerable<CardModel> stashedCards,
        IEnumerable<CardModel> overflowCards)
    {
        foreach (var card in stashedCards)
        {
            if (card.Owner.Creature != Owner || card.Type is not (CardType.Status or CardType.Curse) ||
                card.Pile?.Type != StashPile.Stash) continue;
            await CardCmdCompatibility.Exhaust(ctx, card);
            var enemies = card.Owner.Creature.CombatState?.HittableEnemies;
            if (enemies == null) return;
            Flash();
            await CreatureCmd.Damage(ctx, enemies, Amount, DamageProps.nonCardUnpowered, Owner);
        }
    }
}