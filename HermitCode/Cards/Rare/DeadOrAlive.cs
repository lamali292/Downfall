using BaseLib.Utils;
using Hermit.HermitCode.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Rooms;

namespace Hermit.HermitCode.Cards.Rare;

/// <summary>
///     Deal {Damage} damage X times. If Fatal, get a Bounty. Exhaust.
///     Upgrade: 14 damage.
/// </summary>
public sealed class DeadOrAlive : HermitCardModel
{
    private const int DamageAmount = 8;
    private const int UpgradedDamageAmount = 11;

    private const int MonsterGoldAmount = 15;
    private const int EliteGoldAmount = 40;
    private const int BossGoldAmount = 100;

    public DeadOrAlive() : base(1, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
    {
        WithDamage(8, 3);
        WithKeyword(CardKeyword.Exhaust);
    }

    protected override bool HasEnergyCostX => true;

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay play)
    {
        var times = EnergyCost.CapturedXValue;

        for (var i = 0; i < times; i++)
        {
            await CreatureCmd.TriggerAnim(Owner.Creature, "Attack", Owner.Character.AttackAnimDelay);
            await CommonActions.CardAttack(this, play).WithHermitBluntLightHitFx()
                .Execute(ctx);

            // Stop if target died
            if (play.Target?.IsDead == true) break;
        }

        // If Fatal (target died), gain gold and track Bounty
        var shouldTriggerFatal = play.Target!.Powers.All(p => p.ShouldOwnerDeathTriggerFatal());
        if (play.Target?.IsDead == true && shouldTriggerFatal)
        {
            var currentRoom = Owner.Creature.CombatState?.RunState.CurrentRoom;

            ArgumentNullException.ThrowIfNull(currentRoom);
            var goldAmount = currentRoom.RoomType switch
            {
                RoomType.Monster => MonsterGoldAmount,
                RoomType.Elite => EliteGoldAmount,
                RoomType.Boss => BossGoldAmount,
                _ => throw new InvalidOperationException("Invalid room type for Dead Or Alive card.")
            };
            await PlayerCmd.GainGold(goldAmount, Owner);
        }
    }
}

/* transform_cards.py changes:
 *   namespace → Hermit.HermitCode.Cards.Rare
 *   usings updated
 *   CanonicalVars removed → With* calls in constructor
 *   AdditionalHoverTips/ExtraHoverTips removed (covered by WithPower)
 *   CanonicalKeywords removed → WithKeyword(...) in constructor
 *   OnUpgrade removed (all logic migrated to constructor)
 *   constructor: WithDamage(8, 3), WithKeyword(CardKeyword.Exhaust)
 *   DamageCmd.Attack chain → CommonActions.CardAttack
 */