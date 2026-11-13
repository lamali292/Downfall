using Downfall.DownfallCode.Compatibility;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using SlimeBoss.SlimeBossCode.Core;
using SlimeBoss.SlimeBossCode.Events;
using SlimeBoss.SlimeBossCode.Slimes;

namespace SlimeBoss.SlimeBossCode.Powers;

/// <summary>Slime counterpart of DoubleDamagePower: the owner's Slimes deal double damage and double their non-damage effects until the owner's turn ends.</summary>
public class SlimeDoubleDamagePower : SlimeBossPowerModel, IModifyDamageMultiplicative, IModifySlimeEffects
{
    public int ModifySlimeEffectsMultiplicative(SlimeModel slime)
    {
        return slime.PetOwner == Owner ? 2 : 1;
    }

    public decimal ModifyDamageMultiplicativeCompability(Creature? target, decimal amount, ValueProp props,
        Creature? dealer, CardModel? cardSource, CardPlay? cardPlay)
    {
        return dealer?.Monster is SlimeModel slime && slime.PetOwner == Owner ? 2m : 1m;
    }

    public override async Task AfterSideTurnEnd(PlayerChoiceContext ctx, CombatSide side,
        IEnumerable<Creature> participants)
    {
        if (participants.Contains(Owner)) await PowerCmd.Decrement(this);
    }
}
