using BaseLib.Utils;
using HermitMod.Utility;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Hermit.HermitCode.Cards.Uncommon;

/// <summary>
///     Exhaust a card. Deal 9 damage. If you Exhaust a Curse, deal damage to ALL enemies instead.
///     Upgrade: 12 damage.
/// </summary>
public sealed class Malice : HermitCardModel
{
    private const int DamageAmount = 16;
    private const int UpgradedDamageAmount = 20;

    public Malice() : base(2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        WithDamage(16, 4);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay play)
    {
        // Prompt the player to exhaust a card from hand
        var card = (await CardSelectCmd.FromHand(
            prefs: new CardSelectorPrefs(CardSelectorPrefs.ExhaustSelectionPrompt, 1), context: ctx, player: Owner,
            filter: null, source: this)).FirstOrDefault();
        if (card != null)
            await CardCmd.Exhaust(ctx, card);

        await CreatureCmd.TriggerAnim(Owner.Creature, "Attack", Owner.Character.AttackAnimDelay);
        HermitSfx.PlayGun1();

        if (card?.Type == CardType.Curse)
            // Exhausted a Curse — deal damage to ALL enemies
            await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
                .FromCard(this)
                .TargetingAllOpponents(CombatState!)
                .WithHermitFireHitFx()
                .Execute(ctx);
        else
            // Normal — deal damage to the single target
            await CommonActions.CardAttack(this, play)
                .WithHermitGunHitFx()
                .Execute(ctx);
    }
}

/* transform_cards.py changes:
 *   namespace → Hermit.HermitCode.Cards.Uncommon
 *   usings updated
 *   CanonicalVars removed → With* calls in constructor
 *   AdditionalHoverTips/ExtraHoverTips removed (covered by WithPower)
 *   OnUpgrade removed (all logic migrated to constructor)
 *   constructor: WithDamage(16, 4)
 *   DamageCmd.Attack chain → CommonActions.CardAttack
 */