using Downfall.DownfallCode.Powers;
using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using SlimeBoss.SlimeBossCode.Core;
using MegaCrit.Sts2.Core.ValueProps;
using SlimeBoss.SlimeBossCode.DynamicVars;
using SlimeBoss.SlimeBossCode.Events;
using SlimeBoss.SlimeBossCode.Extensions;
using SlimeBoss.SlimeBossCode.Vfx;

namespace SlimeBoss.SlimeBossCode.Slimes;

/// <summary>Throws a fireball applying Soulburn when Commanded, and is Commanded whenever its owner plays a Power.</summary>
public class PsychicSlime : SlimeModel
{
    // Same yellow as the Searing Ghostflame, which also applies Soulburn.
    private static readonly Color FireColor = new(1f, 0.9f, 0.1f);

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(6, DamageProps.nonCardUnpowered)
    ];

    protected override string? SkinName => "hex";

    public override async Task Command(PlayerChoiceContext ctx, Creature? forcedTarget = null)
    {
        var target = forcedTarget ?? CombatState.GetOpponentsOf(PetOwner).Where(e => e.IsHittable).ToList()
            .OrderBy(_ => Guid.NewGuid()).FirstOrDefault();
        if (target == null) return;
        SpawnFireball(target);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).Unpowered().FromSlime(this).Targeting(target).Execute(ctx);
    }


    public override async Task BeforeCardPlayed(CardPlay cardPlay)
    {
        var card = cardPlay.Card;
        if (card.Owner.Creature != PetOwner || card.Type != CardType.Power) return;
        await SlimeBossCmd.Command<PsychicSlime>(new BlockingPlayerChoiceContext(), card.Owner, 1, card);
    }

    private void SpawnFireball(Creature target)
    {
        var from = NCombatRoom.Instance?.GetCreatureNode(Creature)?.VfxSpawnPosition ?? Vector2.Zero;
        var to = NCombatRoom.Instance?.GetCreatureNode(target)?.VfxSpawnPosition ?? Vector2.Zero;
        NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(NSlimeFireball.Create(from, to, FireColor));
    }
}
