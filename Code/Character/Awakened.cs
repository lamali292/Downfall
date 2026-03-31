using Downfall.Code.Abstract;
using Downfall.Code.Cards.Awakened.Basic;
using Downfall.Code.Commands;
using Downfall.Code.Displays;
using Downfall.Code.Powers.Awakened;
using Downfall.Code.Relics.Awakened;
using Downfall.Code.Vfx;
using Godot;
using MegaCrit.Sts2.Core.Animation;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Characters;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Rooms;

namespace Downfall.Code.Character;


public class Awakened : DownfallCharacterModel
{
    private static readonly Color Color = new(0x12FAF0FF);
    public override string CharId => "Awakened";
    public override Color NameColor => Color;
    public override Color LabOutlineColor => Color;
    public override Color DeckEntryCardColor => Color;
    public override Color CardColor => Color;
    public override Color MapDrawingColor => Color;

    public override CharacterGender Gender => CharacterGender.Neutral;
    protected override CharacterModel? UnlocksAfterRunAs => null;
    public override int StartingHp => 72;
    public override int StartingGold => 99;

    public override IEnumerable<CardModel> StartingDeck =>
    [
        ModelDb.Card<StrikeAwakened>(),
        ModelDb.Card<StrikeAwakened>(),
        ModelDb.Card<StrikeAwakened>(),
        ModelDb.Card<StrikeAwakened>(),
        ModelDb.Card<DefendAwakened>(),
        ModelDb.Card<DefendAwakened>(),
        ModelDb.Card<DefendAwakened>(),
        ModelDb.Card<DefendAwakened>(),
        ModelDb.Card<Hymn>(),
        ModelDb.Card<TalonRake>()
    ];


    public override IReadOnlyList<RelicModel> StartingRelics =>
    [
        ModelDb.Relic<RippedDoll>()
    ];

    public override float AttackAnimDelay => 0.15f;

    public override float CastAnimDelay => 0.25f;

    public override CardPoolModel CardPool => ModelDb.CardPool<AwakenedCardPool>();
    public override PotionPoolModel PotionPool => ModelDb.PotionPool<AwakenedPotionPool>();
    public override RelicPoolModel RelicPool => ModelDb.RelicPool<AwakenedRelicPool>();


    public override CreatureAnimator GenerateAnimator(MegaSprite controller)
    {
        var idleState = new AnimState("Idle_1", true);
        var hitState = new AnimState("Hit");
        var attackState = new AnimState("Attack_1");
        var awakenedIdle = new AnimState("Idle_2", true);
        var awakenedAttack = new AnimState("Attack_2");
        var awakenedHit = new AnimState("Hit");

        hitState.NextState = idleState;
        attackState.NextState = idleState;
        awakenedAttack.NextState = awakenedIdle;
        awakenedHit.NextState = awakenedIdle;

        var animator = new CreatureAnimator(idleState, controller);
        animator.AddAnyState("Idle", idleState, () => !IsAwakened());
        animator.AddAnyState("Idle", awakenedIdle, IsAwakened);
        animator.AddAnyState("Attack", attackState, () => !IsAwakened());
        animator.AddAnyState("Attack", awakenedAttack, IsAwakened);
        animator.AddAnyState("Hit", hitState, () => !IsAwakened());
        animator.AddAnyState("Hit", awakenedHit, IsAwakened);

        return animator;

        bool IsAwakened()
        {
            return CombatManager.Instance.DebugOnlyGetState()
                ?.Players.FirstOrDefault(p => p.Character == this)
                ?.Creature.HasPower<AwakenedFormPower>() ?? false;
        }
    }
    
    
    public override bool ShouldReceiveCombatHooks => true;

    public override Task AfterRoomEntered(AbstractRoom room)
    {
        var state = CombatManager.Instance.DebugOnlyGetState();
        if (state == null) return Task.CompletedTask;
        foreach (var player in state.Players)
        {
            if (player.Character is not Awakened) continue;
            AwakenedCmd.GetSpellbook(player)?.Refresh(player);
            TaskHelper.RunSafely(PowerCmd.Apply<AwakenMeterPower>(player.Creature, 1, player.Creature, null, true));
            var combatRoomNode = NCombatRoom.Instance;
            if (combatRoomNode != null)
            {
                SetupAwakenedUi(combatRoomNode, player);
            }
        }

        return Task.CompletedTask;
    }

    private static void SetupAwakenedUi(NCombatRoom combatRoom, Player player)
    {
        var display = NSpellbookDisplay.Create(player);
        var vfxContainer = combatRoom.CombatVfxContainer;
        vfxContainer.AddChildSafely(display);
        var creatureNode = combatRoom.GetCreatureNode(player.Creature);
        if (creatureNode != null)
        {
            var globalTopPos = creatureNode.GetTopOfHitbox();
            display.Position = vfxContainer.GetGlobalTransform().AffineInverse() * globalTopPos;
            display.Position += new Vector2(-120f, -80f);
        }
        AwakenedDisplay.Register(player, display);
        display.Refresh();
    }
    
    
    
    
}