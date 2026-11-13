using BaseLib.Utils;
using Downfall.DownfallCode.Commands;
using Hexaghost.HexaghostCode.CustomEnums;
using Hexaghost.HexaghostCode.Events;
using Hexaghost.HexaghostCode.History;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.TestSupport;

namespace Hexaghost.HexaghostCode.Core;

public static class HexaghostCmd
{
    public static GhostflameModel[] GetWheel(Player player)
    {
        return HexaghostModel.Wheel.Get(player) ?? [];
    }

    /// <summary>
    ///     All Afterlife-keyworded cards available to the player. Hexaghost players draw only from
    ///     their own pool; other characters draw Afterlife cards from every pool.
    /// </summary>
    public static IEnumerable<CardModel> GetAfterlifeCards(Player player, int amount)
    {
        return DownfallCardCmd.GetSpecificCards<Hexaghost>(player, c => c.Keywords.Contains(HexaghostKeyword.Afterlife),
            amount);
    }


 

    public static void ActivateGhostwheel(Player player)
    {
        HexaghostModel.Active.Set(player, true);
    }

    public static bool IsGhostwheelActivated(Player player)
    {
        return HexaghostModel.Active.Get(player);
    }

    public static int GetCurrentIndex(Player player)
    {
        return HexaghostModel.CurrentIndex.Get(player);
    }

    public static GhostflameModel GetCurrentFlame(Player player)
    {
        return GetWheel(player)[GetCurrentIndex(player)];
    }


    public static T? GetFlameOfType<T>(Player player) where T : GhostflameModel
    {
        return GetWheel(player).OfType<T>().FirstOrDefault();
    }

    public static int GetIgnitedCount(Player player)
    {
        return GetWheel(player).Count(f => f.IsIgnited);
    }

    /// <summary>
    /// Ignited flames as seen by <paramref name="card" /> while it is played or previewed: the flames that are
    /// ignited plus the current flame if this card's own play will ignite it (primed). Flames ignite after the card
    /// resolved, so the card reads the count it is about to cause without that flame's effect firing early.
    /// </summary>
    public static int GetIgnitedCount(Player player, CardModel card)
    {
        return GetIgnitedCount(player) + (GetCurrentFlame(player).IsPrimedBy(card) ? 1 : 0);
    }

    public static bool AllIgnited(Player player)
    {
        return GetWheel(player).All(f => f.IsIgnited);
    }


    private static int GetPreviousIndex(Player player)
    {
        var wheel = GetWheel(player);
        return (GetCurrentIndex(player) + wheel.Length - 1) % wheel.Length;
    }

    private static int GetNextIndex(Player player)
    {
        var wheel = GetWheel(player);
        return (GetCurrentIndex(player) + 1) % wheel.Length;
    }

    public static async Task Advance(PlayerChoiceContext ctx, Player player, AbstractModel? source, bool endOfTurnAdvance = false)
    {
        if (TestMode.IsOff) SfxCmd.Play("event:/sfx/characters/hexaghost-hexaghost/advance");
        await MoveTo(player, GetNextIndex(player));
        var combatState = player.Creature.CombatState!;
        CombatManager.Instance.History.Add(combatState,
            new WheelAdvanceEntry(source, endOfTurnAdvance, player.Creature, combatState.RoundNumber,
                player.Creature.Side, CombatManager.Instance.History, combatState.Players));
        await HexaghostHook.AfterWheelAdvance(player.Creature.CombatState!, ctx, player, source,
            GetCurrentFlame(player),
            GetCurrentIndex(player), endOfTurnAdvance);
    }

    /// <summary>
    /// The flame that was current when <paramref name="card" /> started its current play: the flame the card is
    /// played on, even if the card itself moved the wheel since. Undoes the card's own Advance/Retract entries after
    /// its latest play start in the combat history, so there is no state to go stale (and Replay is covered).
    /// </summary>
    public static GhostflameModel GetFlameBeforePlay(CardModel card)
    {
        var player = card.Owner;
        var shift = 0;
        foreach (var entry in CombatManager.Instance.History.Entries)
            switch (entry)
            {
                case CardPlayStartedEntry started when started.CardPlay.Card == card:
                    shift = 0;
                    break;
                case WheelAdvanceEntry { IsEndOfTurnAdvance: false } advance when advance.Source == card:
                    shift++;
                    break;
                case WheelRetractEntry retract when retract.Source == card:
                    shift--;
                    break;
            }

        var wheel = GetWheel(player);
        var index = ((GetCurrentIndex(player) - shift) % wheel.Length + wheel.Length) % wheel.Length;
        return wheel[index];
    }

    /// <summary>
    /// Whether <paramref name="card" /> ignited <paramref name="flame" /> by hand during its current play while its
    /// play would also have ignited it afterwards (read from the combat history after the play start, like
    /// <see cref="GetFlameBeforePlay" />).
    /// </summary>
    public static bool WasPrimedAndIgnitedByPlay(CardModel card, GhostflameModel flame)
    {
        var result = false;
        foreach (var entry in CombatManager.Instance.History.Entries)
            if (entry is CardPlayStartedEntry started && started.CardPlay.Card == card) result = false;
            else if (entry is GhostflameIgniteEntry ignite && ignite.Source == card && ignite.Flame == flame &&
                     ignite.WasPrimedBySource) result = true;
        return result;
    }

    public static async Task Retract(PlayerChoiceContext ctx, Player player, AbstractModel? source)
    {
        if (TestMode.IsOff) SfxCmd.Play("event:/sfx/characters/hexaghost-hexaghost/retract");
        await MoveTo(player, GetPreviousIndex(player));
        var combatState = player.Creature.CombatState!;
        CombatManager.Instance.History.Add(combatState,
            new WheelRetractEntry(source, player.Creature, combatState.RoundNumber,
                player.Creature.Side, CombatManager.Instance.History, combatState.Players));
        await HexaghostHook.AfterWheelRetract(player.Creature.CombatState!, ctx, player, source,
            GetCurrentFlame(player),
            GetCurrentIndex(player));
    }

    /// <summary>How many Advances/Retracts the player performed this turn (the end-of-turn advance excluded).</summary>
    public static int CountWheelMovesThisTurn(Creature creature)
    {
        var combatState = creature.CombatState;
        return CombatManager.Instance.History.Entries.OfType<WheelMoveEntry>()
            .Count(e => e is not WheelAdvanceEntry { IsEndOfTurnAdvance: true } && e.Actor == creature && e.HappenedThisTurn(combatState));
    }
    

    public static Task ReplaceCurrentWithRandom(Player player)
    {
        var wheel = GetWheel(player);
        var currentIdx = GetCurrentIndex(player);
        var rng = player.RunState.Rng.Niche;

        var current = wheel[currentIdx];
        var isOffclass = current.IsOffclass;
        var currentType = current.GetType();
        var candidates = HexaghostModelDb.AllGhostflames
            .Where(f => f.GetType() != currentType && f.IsOffclass == isOffclass).ToArray();
        var randomFlame = rng.NextItem(candidates);

        if (randomFlame == null) return Task.CompletedTask;
        wheel[currentIdx] = randomFlame.ToMutable(player);
        Refresh(player);
        return Task.CompletedTask;
    }


    private static Task MoveTo(Player player, int index)
    {
        if (player.PlayerCombatState == null) return Task.CompletedTask;
        ActivateGhostwheel(player);
        HexaghostModel.CurrentIndex[player] = index;
        var flame = GetCurrentFlame(player);
        flame.Extinguish();
        flame.UpdateVisuals();
        Refresh(player);
        return Task.CompletedTask;
    }

    public static bool IsIgnited(Player player)
    {
        return GetCurrentFlame(player).IsIgnited;
    }

    /// <summary>Whether the current flame is ignited or primed by <paramref name="card" />'s own play (see <see cref="GetIgnitedCount(Player, CardModel)" />).</summary>
    public static bool IsIgnited(Player player, CardModel card)
    {
        var flame = GetCurrentFlame(player);
        return flame.IsIgnited || flame.IsPrimedBy(card);
    }

    public static bool IsPreviousIgnited(Player player)
    {
        return GetWheel(player)[GetPreviousIndex(player)].IsIgnited;
    }

    public static bool IsNextIgnited(Player player)
    {
        return GetWheel(player)[GetNextIndex(player)].IsIgnited;
    }

    public static Task IgnitePrevious(PlayerChoiceContext ctx, Player player)
    {
        return IgniteAt(ctx, player, GetPreviousIndex(player));
    }

    public static Task IgniteNext(PlayerChoiceContext ctx, Player player)
    {
        return IgniteAt(ctx, player, GetNextIndex(player));
    }

    public static Task Ignite(PlayerChoiceContext ctx, Player player)
    {
        return IgniteAt(ctx, player, GetCurrentIndex(player));
    }

    /// <summary>
    /// Ignites a flame by hand. When this happens during a card's play and that play would also ignite the flame once
    /// it resolved (primed), the ignition is recorded in the combat history so the flame still ignites a second time
    /// after the play (see <see cref="WasPrimedAndIgnitedByPlay" />). Primed is only a pre-state: it never replaces
    /// an ignition the card does itself.
    /// </summary>
    public static async Task IgniteAt(PlayerChoiceContext ctx, Player player, int index)
    {
        ActivateGhostwheel(player);
        await Cmd.Wait(0.05f);
        var flame = GetWheel(player)[index];
        if (GetCardBeingPlayed(player) is { } playing)
        {
            var combatState = player.Creature.CombatState!;
            CombatManager.Instance.History.Add(combatState,
                new GhostflameIgniteEntry(flame, playing, flame.WouldIgniteFrom(playing), player.Creature,
                    combatState.RoundNumber, player.Creature.Side, CombatManager.Instance.History,
                    combatState.Players));
        }
        if (!flame.IsIgnited)
            flame.IsIgnited = true;

        var allIgnited = AllIgnited(player);
        flame.SetIgniteProgress();
        Refresh(player);
        await flame.OnIgnite(ctx);
        await HexaghostHook.AfterGhostwheelIgnited(player.Creature.CombatState!, ctx, player, flame, index);
        await Cmd.Wait(0.05f);
        if (allIgnited)
            await HexaghostHook.AfterGhostwheelAllIgnited(player.Creature.CombatState!, ctx, player, flame, index);
    }


    public static async Task IgniteAll(PlayerChoiceContext ctx, Player player)
    {
        var wheel = GetWheel(player);
        for (var i = 0; i < wheel.Length; i++) await IgniteAt(ctx, player, i);
    }

    public static Task ExtinguishAllExceptThis(PlayerChoiceContext ctx, Player player, GhostflameModel model)
    {
        foreach (var f in GetWheel(player).Where(e => e != model))
            f.Extinguish();
        Refresh(player);
        return Task.CompletedTask;
    }


    public static Task Extinguish(Player player, bool silent = false)
    {
        GetCurrentFlame(player).Extinguish();
        if (silent) return Task.CompletedTask;
        Refresh(player);
        return Task.CompletedTask;
    }

    /// <param name="startingCard">A card whose play is just starting and not yet in the combat history.</param>
    public static void Refresh(Player player, CardModel? startingCard = null)
    {
        if (!IsGhostwheelActivated(player)) return;
        HexaghostVisualsBridge.Refresh(player, startingCard);
    }

    /// <summary>
    /// The flame that shows as ignited only because the card being played right now will ignite it after the play
    /// (null when none). <paramref name="startingCard" /> covers the moment before the play is in the combat history.
    /// </summary>
    /// <summary>The card of <paramref name="player" /> between its play start and finish in the combat history, if any.</summary>
    public static CardModel? GetCardBeingPlayed(Player player)
    {
        CardModel? playing = null;
        foreach (var entry in CombatManager.Instance.History.Entries)
            if (entry is CardPlayStartedEntry started && started.CardPlay.Card.Owner == player)
                playing = started.CardPlay.Card;
            else if (entry is CardPlayFinishedEntry finished && finished.CardPlay.Card == playing)
                playing = null;
        return playing;
    }

    public static GhostflameModel? GetPrimedFlameForVisuals(Player player, CardModel? startingCard)
    {
        if (startingCard != null)
            return GetCurrentFlame(player).WouldIgniteFrom(startingCard) ? GetCurrentFlame(player) : null;
        var playing = GetCardBeingPlayed(player);
        if (playing == null) return null;
        var flame = GetFlameBeforePlay(playing);
        return flame.WouldIgniteFrom(playing) ? flame : null;
    }

    public static Task<int> ResetWheel(Player player)
    {
        Cmd.Wait(0.1f);
        var a = GetWheel(player).Count(flame => flame.Extinguish());
        Cmd.Wait(0.1f);
        HexaghostModel.ResetWheel(player);
        Cmd.Wait(0.1f);
        Refresh(player);
        return Task.FromResult(a);
    }

    public static void SetCurrentGhostflame(Player player, GhostflameModel ghostflame)
    {
        ghostflame.AssertCanonical();
        ActivateGhostwheel(player);
        GetWheel(player)[GetCurrentIndex(player)] = ghostflame.ToMutable(player);
        Refresh(player);
    }

    public static AttackCommand AfterlifeAttack(CardModel card, CardPlay? cardPlay)
    {
        AttackCommand a;
        if (card.DynamicVars.ContainsKey("CalculatedDamage"))
            a = DamageCmd.Attack(card.DynamicVars.CalculatedDamage);
        else if (card.DynamicVars.ContainsKey("Damage"))
            a = DamageCmd.Attack(card.DynamicVars.Damage.BaseValue);
        else
            throw new Exception(
                $"Card {card.Title} does not have a damage variable supported by CommonActions.CardAttack");
        a = a.FromCardCompatibility(card, cardPlay);
        if (cardPlay?.Target != null) return a.Targeting(cardPlay.Target);
        if (card.CombatState != null)
            return card.TargetType == TargetType.AllEnemies
                ? a.TargetingAllOpponents(card.CombatState)
                : a.TargetingRandomOpponents(card.CombatState);
        throw new InvalidOperationException("Afterlife attack failed!");
    }
}