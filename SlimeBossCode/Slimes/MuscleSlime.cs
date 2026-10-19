using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using SlimeBoss.SlimeBossCode.Core;
using SlimeBoss.SlimeBossCode.CustomEnums;
using SlimeBoss.SlimeBossCode.Extensions;

namespace SlimeBoss.SlimeBossCode.Slimes;

public class MuscleSlime : SlimeModel
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(5,DamageProps.nonCardUnpowered)
    ];

    protected override string? SkinName => "attack";

    public override async Task Command(PlayerChoiceContext ctx, Creature? forcedTarget = null)
    {
        var attack = DamageCmd.Attack(DynamicVars.Damage.BaseValue).Unpowered().FromSlime(this);
        attack = forcedTarget != null ? attack.Targeting(forcedTarget) : attack.TargetingRandomOpponents(CombatState);
        await attack.Execute(ctx);
    }
    
    public override async Task AfterCardPlayed(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var card = cardPlay.Card;
        if (card.Owner.Creature != PetOwner || !card.Tags.Contains(SlimeBossTag.Tackle)) return;
        await SlimeBossCmd.Command<MuscleSlime>(ctx, card.Owner, 1, card);
    }
}
