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
        new SlimePowerVar<SoulBurnPower>(6)
    ];

    public override IEnumerable<IHoverTip> ExtraTips => [HoverTipFactory.FromPower<SoulBurnPower>()];

    public override string CounterIconPath => "res://Downfall/images/powers/soul_burn_power.png";

    protected override string? SkinName => "hex";

    public override async Task Command(PlayerChoiceContext ctx, Creature? forcedTarget = null)
    {
        var target = forcedTarget ?? CombatState.GetOpponentsOf(PetOwner).Where(e => e.IsHittable).ToList()
            .OrderBy(_ => Guid.NewGuid()).FirstOrDefault();
        if (target == null) return;
        var amount = SlimeBossHook.ModifySlimeEffects(CombatState, DynamicVars.Slime.IntValue, out _, this);
        SpawnFireball(target);
        // The player is the applier, so Red Candle's bonus applies.
        await PowerCmd.Apply<SoulBurnPower>(ctx, target, amount, PetOwner, null);
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
