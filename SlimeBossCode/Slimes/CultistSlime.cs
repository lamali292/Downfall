using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using Downfall.DownfallCode.DynamicVars;
using SlimeBoss.SlimeBossCode.Extensions;

namespace SlimeBoss.SlimeBossCode.Slimes;

public class CultistSlime : SlimeModel
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        ..CustomModelCalculatedDamageVar.Create("Damage", DamageProps.nonCardUnpowered, 4, PowersPlayedThisCombat)
    ];

    private static decimal PowersPlayedThisCombat(ICustomAbstractModel model, Creature? _) => CombatManager.Instance.History.Entries
        .OfType<CardPlayStartedEntry>()
        .Count(e => e.CardPlay.Card.Type == CardType.Power && e.CardPlay.Card.Owner.Creature == ((SlimeModel)model).PetOwner);

    protected override string SkinName => "cultist";

    public override async Task Command(PlayerChoiceContext ctx, Creature? forcedTarget = null)
    {
        var damage = ((CustomModelCalculatedDamageVar)DynamicVars["Damage"]).CalculateCustom(forcedTarget);
        var attack = DamageCmd.Attack(damage).FromSlime(this);
        attack = forcedTarget != null ? attack.Targeting(forcedTarget) : attack.TargetingRandomOpponents(CombatState);
        await attack.Execute(ctx);
    }
}
