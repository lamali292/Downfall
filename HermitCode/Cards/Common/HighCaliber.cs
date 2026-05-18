using BaseLib.Utils;
using Downfall.DownfallCode.Commands;
using Hermit.HermitCode.Cards.Basic;
using Hermit.HermitCode.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Enchantments;

namespace Hermit.HermitCode.Cards.Common;

/// <summary>
///     Deal 6 damage. Add a Strike+2 to your hand. Exhaust.
///     Upgrade: 9 damage and Strike+3.
/// </summary>
public sealed class HighCaliber : HermitCardModel
{
    public HighCaliber() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
        WithDamage(6, 3);
        WithKeyword(CardKeyword.Exhaust);
        WithUpgradingCardTip<StrikeHermit>();
        WithVar("SharpAmount", 6);
        WithTip(typeof(Sharp));
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Attack", Owner.Character.AttackAnimDelay);
        HermitSfx.PlayGun1();
        await CommonActions.CardAttack(this, play).WithHermitGunHitFx()
            .Execute(ctx);
        await DownfallCardCmd.GiveCard<StrikeHermit>(Owner, PileType.Hand, upgraded: IsUpgraded, action: card =>
        {
            CardCmd.Enchant<Sharp>(card, DynamicVars["SharpAmount"].BaseValue);
        });
    }
}

/* transform_cards.py changes:
 *   namespace → Hermit.HermitCode.Cards.Common
 *   usings updated
 *   CanonicalKeywords removed → WithKeyword(...) in constructor
 *   OnUpgrade removed (all logic migrated to constructor)
 *   constructor: WithDamage(6, 3), WithKeyword(CardKeyword.Exhaust)
 *   DamageCmd.Attack chain → CommonActions.CardAttack
 */