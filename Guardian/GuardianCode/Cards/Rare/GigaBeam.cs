using BaseLib.Utils;
using Downfall.DownfallCode.Powers;

using Guardian.GuardianCode.Core;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.ValueProps;

namespace Guardian.GuardianCode.Cards.Rare;

[Pool(typeof(GuardianCardPool))]
public class GigaBeam : GuardianCardModel
{
    public GigaBeam() : base(3, CardType.Attack, CardRarity.Rare, TargetType.AllEnemies)
    {
        WithDamage(36, 4);
        WithVar("StrengthEffect", 2, 2);
        WithPower<StunnedPower>(1);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (CombatState == null) return;
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .TargetingAllOpponents(CombatState)
            .WithAttackerAnim("Cast", 0.5f)
            .BeforeDamage(BeforeDamageAction)
            .Execute(ctx);
        await CommonActions.ApplySelf<StunnedPower>(ctx, this);
    }

    private async Task BeforeDamageAction()
    {
        var enemies = CombatState?.Enemies.Where(e => e.IsAlive).ToList();

        if (enemies != null)
        {
            var vfx = NHyperbeamVfx.Create(Owner.Creature, enemies.Last());
            if (vfx != null) NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(vfx);
        }

        await Cmd.Wait(0.5f);
        if (enemies != null)
            foreach (var impact in enemies
                         .Select(enemy => NHyperbeamImpactVfx.Create(Owner.Creature, enemy))
                         .OfType<NHyperbeamImpactVfx>())
                NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(impact);
    }

    public override decimal ModifyDamageAdditive(
        Creature? target,
        decimal amount,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource)
    {
        return cardSource != this || !props.IsPoweredAttack()
            ? 0M
            : dealer?.GetPowerAmount<StrengthPower>() * (DynamicVars["StrengthEffect"].IntValue - 1) ?? 0;
    }
}