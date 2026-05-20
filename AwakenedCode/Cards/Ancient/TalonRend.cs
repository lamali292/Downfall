using Awakened.AwakenedCode.Core;
using Awakened.AwakenedCode.CustomEnums;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Awakened.AwakenedCode.Cards.Ancient;

[Pool(typeof(AwakenedCardPool))]
public class TalonRend : AwakenedCardModel
{
    public TalonRend() : base(1, CardType.Attack, CardRarity.Ancient, TargetType.AnyEnemy)
    {
        WithDamage(5, 3);
        WithConjure();
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(CombatState);
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target)
            .WithHitCount(2)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(ctx);

        await AwakenedCmd.Conjure(Owner, CombatState);
        await AwakenedCmd.Conjure(Owner, CombatState);
    }
}