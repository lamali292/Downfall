using BaseLib.Utils;
using Hermit.HermitCode.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Hermit.HermitCode.Cards.Rare;

/// <summary>
///     Discard 6 cards. For each card discarded, deal 8 damage.
///     Upgrade: 12 damage per card.
/// </summary>
public sealed class Magnum : HermitCardModel
{
    public Magnum() : base(1, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
    {
        WithDamage(6, 2);
        WithCards(6);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay play)
    {
        // Prompt player to discard up to 6 cards
        var handCount = PileType.Hand.GetPile(Owner).Cards.Count;
        var maxDiscard = Math.Min(DynamicVars.Cards.IntValue, handCount);

        if (maxDiscard > 0)
        {
            var selected = (await CardSelectCmd.FromHandForDiscard(
                ctx,
                Owner,
                new CardSelectorPrefs(CardSelectorPrefs.DiscardSelectionPrompt, maxDiscard, maxDiscard),
                null,
                this
            )).ToList();

            if (selected.Count > 0)
            {
                await CardCmd.Discard(ctx, selected);

                await CreatureCmd.TriggerAnim(Owner.Creature, "Attack", Owner.Character.AttackAnimDelay);
                HermitSfx.PlayGun1();

                // Deal damage once per card discarded
                for (var i = 0; i < selected.Count; i++)
                {
                    if (play.Target?.IsDead == true) break;
                    await CommonActions.CardAttack(this, play).WithHermitGunHitFx()
                        .Execute(ctx);
                }
            }
        }
    }
}

/* transform_cards.py changes:
 *   namespace → Hermit.HermitCode.Cards.Rare
 *   usings updated
 *   CanonicalVars removed → With* calls in constructor
 *   OnUpgrade removed (all logic migrated to constructor)
 *   constructor: WithDamage(6, 2)
 *   DamageCmd.Attack chain → CommonActions.CardAttack
 */