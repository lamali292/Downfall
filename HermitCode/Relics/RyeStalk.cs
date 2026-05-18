using Hermit.HermitCode.Core;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Hermit.HermitCode.Relics;

/// <summary>
///     Whenever you lose HP during enemy turn, draw 1 card.
///     Requires Harmony patch on damage system for full implementation.
/// </summary>
public sealed class RyeStalk() : HermitRelicModel(RelicRarity.Common)
{
    public override async Task AfterDamageReceived(PlayerChoiceContext ctx, Creature target, DamageResult result,
        ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (target == Owner.Creature && result.UnblockedDamage > 0 && props.HasFlag(ValueProp.Move) && dealer != null &&
            dealer.IsEnemy)
        {
            Flash();
            await CardPileCmd.Draw(ctx, Owner);
        }
    }
}