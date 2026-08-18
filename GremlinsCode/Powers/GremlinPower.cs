using BaseLib.Abstracts;
using BaseLib.Patches.Localization;
using Downfall.DownfallCode.Commands;
using Downfall.DownfallCode.Compatibility;
using Downfall.DownfallCode.Powers;
using Gremlins.GremlinsCode.Cards.Token;
using Gremlins.GremlinsCode.Core;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace Gremlins.GremlinsCode.Powers;

public class GremlinPower()
    : GremlinsPowerModel(PowerType.Buff, PowerStackType.Single), IAddDumbVariablesToPowerDescription, IModifyDamageAdditive
{
    private MonsterModel? CurrentGremlinMonster =>
        IsMutable ? GremlinsCmd.GetCurrentGremlin(Owner.Player)?.Monster : null;

    private string GremlinName => CurrentGremlinMonster?.GetType().Name ?? "None";

    public void AddDumbVariablesToPowerDescription(LocString description)
    {
        description.Add("GremlinVariant", GremlinName);
    }

    public override async Task BeforeHandDraw(Player player, PlayerChoiceContext ctx, ICombatState combatState)
    {
        if (Owner != player.Creature) return;
        var gremlin = GremlinsCmd.GetCurrentGremlin(player);
        if (gremlin?.Monster is not GremlinsMonsterModel monster) return;
        if (monster is not GremlinNob) return;
        await DownfallCardCmd.GiveCard<Bellow>(player, PileType.Hand);
        await DownfallCardCmd.GiveCard<SkullBash>(player, PileType.Hand);
        await DownfallCardCmd.GiveCard<Rush>(player, PileType.Hand);
    }


    protected override async Task BeforeCardPlayed(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var player = cardPlay.Card.Owner;
        if (Owner != player.Creature) return;
        var gremlin = GremlinsCmd.GetCurrentGremlin(player);
        if (gremlin?.Monster is not GremlinsMonsterModel monster) return;
        var combatState = cardPlay.Card.CombatState;
        if (combatState == null) return;
        switch (monster)
        {
            case MadGremlin:
                break;
            case FatGremlin:
                if (cardPlay.Card.Type != CardType.Attack) return;
                await PowerCmd.Apply<WeakPower>(ctx, combatState.HittableEnemies, 1, Owner, null);
                break;
            case WizardGremlin:
                if (cardPlay.Card.Type != CardType.Skill) return;
                await PowerCmd.Apply<WizPower>(ctx, Owner, 1, Owner, null);
                break;
            case ShieldGremlin:
                if (cardPlay.Card.Type != CardType.Skill) return;
                await CreatureCmd.GainBlock(Owner, 2, BlockProps.nonCardUnpowered, null);
                break;
            case SneakGremlin:
                if (cardPlay.Card.Type != CardType.Attack) return;
                var randomEnemy = combatState.RunState.Rng.CombatTargets.NextItem(combatState.HittableEnemies);
                if (randomEnemy == null) return;
                await CreatureCmd.Damage(ctx, randomEnemy, 2, DamageProps.nonCardUnpowered, Owner);
                break;
        }
    }

    public decimal ModifyDamageAdditiveCompability(Creature? target, decimal amount, ValueProp props, Creature? dealer,
        CardModel? cardSource, CardPlay? cardPlay)
    {
        return dealer == Owner && cardSource is { EnergyCost.Canonical: 0 } && GremlinsCmd
            .GetCurrentGremlin(Owner.Player)?
            .Monster is SneakGremlin
            ? 2
            : 0;
    }

    public override async Task AfterAttack(PlayerChoiceContext ctx, AttackCommand command)
    {
        var player = Owner.Player;
        if (player == null) return;
        var gremlin = GremlinsCmd.GetCurrentGremlin(player);
        if (gremlin is not { Monster: MadGremlin }) return;
        if (command.Results.SelectMany(e => e).All(e => e.Receiver != player.Creature)) return;
        await PowerCmd.Apply<GremlinStrengthPower>(ctx, player.Creature, 2, player.Creature, null);
    }
}

public class GremlinStrengthPower : CustomTemporaryPowerModelWrapper<GremlinPower, StrengthPower>;