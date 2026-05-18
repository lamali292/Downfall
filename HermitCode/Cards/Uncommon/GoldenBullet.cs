using BaseLib.Utils;
using HermitMod.Utility;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Saves.Runs;

namespace Hermit.HermitCode.Cards.Uncommon;

/// <summary>
///     Deal 20 damage. If Fatal, permanently reduce this card's cost by 1. Exhaust.
///     Upgrade: 28 damage.
/// </summary>
public sealed class GoldenBullet : HermitCardModel
{
    private int _currentCost = 3;

    public GoldenBullet() : base(3, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        WithDamage(18, 6);
        WithKeyword(CardKeyword.Exhaust);
    }

    [SavedProperty]
    private int CurrentCost
    {
        get => _currentCost;
        set
        {
            AssertMutable();
            _currentCost = value;
            EnergyCost.SetCustomBaseCost(_currentCost);
        }
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Attack", Owner.Character.AttackAnimDelay);
        HermitSfx.PlayGun1();

        var shouldTriggerFatal = play.Target!.Powers.All(p => p.ShouldOwnerDeathTriggerFatal());
        var attackCommand = await CommonActions.CardAttack(this, play).WithHermitGunHitFx()
            .Execute(ctx);

        if (shouldTriggerFatal && attackCommand.Results.SelectMany(r => r).Any(r => r.WasTargetKilled))
        {
            BuffFromPlay();
            // Sync to the deck version so the reduction persists after combat
            (DeckVersion as GoldenBullet)?.BuffFromPlay();
        }
    }

    protected override void AfterDowngraded()
    {
        UpdateCost();
    }

    private void BuffFromPlay()
    {
        CurrentCost = Math.Max(0, CurrentCost - 1);
        UpdateCost();
    }

    private void UpdateCost()
    {
        EnergyCost.SetCustomBaseCost(CurrentCost);
    }
}

/* transform_cards.py changes:
 *   namespace → Hermit.HermitCode.Cards.Uncommon
 *   usings updated
 *   CanonicalVars removed → With* calls in constructor
 *   AdditionalHoverTips/ExtraHoverTips removed (covered by WithPower)
 *   CanonicalKeywords removed → WithKeyword(...) in constructor
 *   OnUpgrade removed (all logic migrated to constructor)
 *   constructor: WithDamage(18, 6), WithKeyword(CardKeyword.Exhaust)
 *   DamageCmd.Attack chain → CommonActions.CardAttack
 */