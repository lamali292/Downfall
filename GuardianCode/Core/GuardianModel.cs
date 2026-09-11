using BaseLib.Abstracts;
using BaseLib.Utils;
using Downfall.DownfallCode.Core;
using Guardian.GuardianCode.Cards.Abstract;
using Guardian.GuardianCode.Displays;
using Guardian.GuardianCode.Events;
using Guardian.GuardianCode.Interfaces;
using Guardian.GuardianCode.Piles;
using Guardian.GuardianCode.Powers;
using Guardian.GuardianCode.RestSiteOptions;
using Guardian.GuardianCode.Vfx;
using MegaCrit.Sts2.Core.Animation;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.RestSite;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;

namespace Guardian.GuardianCode.Core;

public class GuardianCombatModel() : CustomSingletonModel(HookType.Combat)
{
    // SpireFields
    internal static readonly PlayerField<GuardianModeModel> ActiveMode =
        new(GuardianModelDb.GuardianMode<GuardianNormalMode>);

    internal static readonly PlayerField<int> StasisSlots = new(() => -1);
    internal static readonly SpireField<CardModel, int> StasisCounter = new(_ => 0);

    // Hooks
    public override async Task BeforeHandDraw(Player player, PlayerChoiceContext ctx, ICombatState combatState)
    {
        if (player is { Character: Guardian, PlayerCombatState.TurnNumber: 1 })
        {
            await PowerCmd.Apply<ModeShiftPower>(ctx, player.Creature, 20, player.Creature, null, true);
            await GuardianCmd.LeaveDefensiveMode(ctx, player);
        }

        await GuardianCmd.TickAll(player, ctx);
        GuardianDisplay.Refresh(player);
    }

    public override Task BeforeCombatStart()
    {
        StasisSlots.Clear();
        ActiveMode.Clear();
        StasisCounter._table.Clear();
        
        foreach (var player in RunManager.Instance.State?.Players ?? [])
        {
            if (StasisSlots[player] < 0)
                StasisSlots.Set(player, player.Character is Guardian ? 3 : 1);
        }
        
        return Task.CompletedTask;
    }

    public override Task AfterCombatEnd(CombatRoom room)
    {
        StasisSlots.Clear();
        ActiveMode.Clear();
        StasisCounter._table.Clear();
        return Task.CompletedTask;
        
    }


    public override Task AfterCardChangedPilesLate(CardModel card, PileType oldPileType, AbstractModel? source)
    {
        if (card.Pile != null && card.Pile.Type != GuardianPile.Stasis) return Task.CompletedTask;
        GuardianDisplay.Refresh(card.Owner);
        return Task.CompletedTask;
    }

    internal static void SetupGuardianCombatUi(CombatState state)
    {
        var combatRoomNode = NCombatRoom.Instance;
        if (combatRoomNode == null) return;
        
        foreach (var player in state.Players)
        {
            if (player.Character is not Guardian) continue;
            InitStasisUi(player);
        }
    }

    internal static GuardianPile GetOrInitStasis(Player player)
    {
        var pile = GuardianCmd.GetStasisPile(player);
        InitStasisUi(player);
        return pile;
    }

    internal static void InitStasisUi(Player player)
    {
       

        var combatRoom = NCombatRoom.Instance;
        if (combatRoom != null && !GuardianDisplay.HasDisplay(player))
            GuardianDisplay.SetupGuardianUi(combatRoom, player);

        GuardianDisplay.Refresh(player);
    }

    internal static async Task SetMode(PlayerChoiceContext ctx, Player player, GuardianModeModel newCanonical)
    {
        var current = ActiveMode[player];
        if (current?.GetType() == newCanonical.GetType()) return;
        if (current != null) await current.OnExit();
        var mutable = newCanonical.ToMutable(player);
        ActiveMode[player] = mutable;
        await mutable.OnEnter();
        if (newCanonical is GuardianDefensiveMode)
        {
            await CreatureCmd.TriggerAnim(player.Creature, "TransitionIn", 0.5f);
        }
        else
        {
            await CreatureCmd.TriggerAnim(player.Creature, "TransitionOut", 0.5f);
        }
     
        await GuardianHook.AfterGuardianModeChangeEarly(player.Creature.CombatState!, ctx, player, current!,
            ActiveMode[player]!);
        await GuardianHook.AfterGuardianModeChange(player.Creature.CombatState!, ctx, player, current!,
            ActiveMode[player]!);
    }
}

public class GuardianRunModel() : CustomSingletonModel(HookType.Run)
{
    public override bool TryModifyRestSiteOptions(Player player, ICollection<RestSiteOption> options)
    {
        if (options.Any(option => option.OptionId == GemRestSiteOption.Id)) return false;

        var deck = player.DeckPile;
        var hasGems = deck.Any(e => e is IGemCard);
        var hasSlots = deck.Any(e => e is IGemSocketCard { FreeSlots: > 0 });
        if (!hasSlots || !hasGems) return false;
        options.Add(new GemRestSiteOption(player));
        return true;
    }
}