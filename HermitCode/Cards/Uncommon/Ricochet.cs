using BaseLib.Utils;
using Hermit.HermitCode.CustomEnums;
using Hermit.HermitCode.History;
using Hermit.HermitCode.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace Hermit.HermitCode.Cards.Uncommon;

public sealed class Ricochet : HermitCardModel
{
    public Ricochet() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        WithDamage(7, 2);
        WithCalculatedVar("CalculatedHits", 0, 1, CountDeadOnEffects);
        WithTip(HermitKeywords.DeadOn);
    }

    private static decimal CountDeadOnEffects(CardModel card, Creature? _)
    {
        return CombatManager.Instance.History.Entries.OfType<DeadOnEntry>().Count(e =>
            e.HappenedThisTurn(card.CombatState) && e.Actor == card.Owner.Creature);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(play.Target);
        await CreatureCmd.TriggerAnim(Owner.Creature, "Attack", Owner.Character.AttackAnimDelay);
        var extraHitCount = (int)((CalculatedVar)DynamicVars["CalculatedHits"]).Calculate(play.Target);
        HermitSfx.PlayGun2();
        await CommonActions.CardAttack(this, play)
            .WithHermitGunHitFx()
            .Execute(ctx);
        for (var i = 0; i < extraHitCount; i++)
        {
            var enemies = CombatState?.HittableEnemies.ToList();
            if (enemies == null || enemies.Count == 0) break;
            HermitSfx.PlayGun3();
            await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
                .FromCard(this)
                .TargetingRandomOpponents(CombatState!)
                .WithHermitGunHitFx()
                .Execute(ctx);
        }
    }
}