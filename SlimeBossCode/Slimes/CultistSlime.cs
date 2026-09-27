using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using SlimeBoss.SlimeBossCode.Cards.Token;
using SlimeBoss.SlimeBossCode.Extensions;

namespace SlimeBoss.SlimeBossCode.Slimes;

public class CultistSlime : SlimeModel
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(4, DamageProps.nonCardUnpowered)
    ];
    
    protected override string? SkinName => "cultist";

    public override async Task Command(PlayerChoiceContext ctx, Creature? forcedTarget = null)
    {
        var attack = DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromSlime(this);
        attack = forcedTarget != null ? attack.Targeting(forcedTarget) : attack.TargetingRandomOpponents(CombatState);
        await attack.Execute(ctx);
    }
    
    public override Task AfterCardPlayed(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner.Creature != PetOwner || cardPlay.Card.Type != CardType.Power) return Task.CompletedTask;

        DynamicVars.Damage.BaseValue += 1;
        return Task.CompletedTask;
    }
}
