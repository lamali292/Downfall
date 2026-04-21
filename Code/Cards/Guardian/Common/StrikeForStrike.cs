using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Abstract.CardModels;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace Downfall.Code.Cards.Guardian.Common;

[Pool(typeof(GuardianCardPool))]
public class StrikeForStrike : GuardianCardModel
{
    public StrikeForStrike() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
        WithDamage(13, 4);
        WithVars(EnemyDamage);
    }

    public override int GemSlots => 1;
    private static DamageVar EnemyDamage => new("EnemyDamage", 3, ValueProp.Move);

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (cardPlay.Target?.Monster == null) return;
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
        if (!cardPlay.Target.IsAlive) return;
        await Cmd.Wait(0.5f);
        var attack = DamageCmd.Attack(EnemyDamage.BaseValue);
        attack.Attacker = cardPlay.Target.Monster.Creature;
        attack._attackerAnimName = "Attack";
        attack._sourceType = AttackCommand.SourceType.Monster;
        await attack
            .Targeting(Owner.Creature)
            .WithHitFx("vfx/vfx_attack_slash", "event:/sfx/characters/silent/silent_attack")
            .Execute(ctx);
    }
}