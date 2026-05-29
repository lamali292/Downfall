using BaseLib.Patches.Content;
using Guardian.GuardianCode.Cards.Abstract;
using Guardian.GuardianCode.CustomEnums;
using Guardian.GuardianCode.Displays;
using Guardian.GuardianCode.Events;
using Guardian.GuardianCode.Extensions;
using Guardian.GuardianCode.Interfaces;
using Guardian.GuardianCode.Piles;
using Guardian.GuardianCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.Vfx;

namespace Guardian.GuardianCode.Core;

public static class GuardianCmd
{
    private static readonly LocString FullStasisText = new("combat_messages", "FULL_STASIS_SLOTS");

    // Mode
    public static Task EnterDefensiveMode(PlayerChoiceContext ctx, Player player)
    {
        return GuardianCombatModel.SetMode(ctx, player, GuardianModelDb.GuardianMode<GuardianDefensiveMode>());
    }

    public static Task LeaveDefensiveMode(PlayerChoiceContext ctx, Player player)
    {
        return GuardianCombatModel.SetMode(ctx, player, GuardianModelDb.GuardianMode<GuardianNormalMode>());
    }

    public static Task ChangeMode(PlayerChoiceContext ctx, Player player)
    {
        return IsInMode<GuardianNormalMode>(player) ? EnterDefensiveMode(ctx, player) : LeaveDefensiveMode(ctx, player);
    }

    public static GuardianModeModel GetMode(Player player)
    {
        return GuardianCombatModel.ActiveMode[player] ?? GuardianModelDb.GuardianMode<GuardianNormalMode>();
    }

    public static bool IsInMode<T>(Player player) where T : GuardianModeModel
    {
        return GuardianCombatModel.ActiveMode[player] is T;
    }

    // Stasis
    public static int GetStasisCount(Player player)
    {
        return TryGetStasisPile(player)?.Cards.Count ?? 0;
    }

    public static IReadOnlyList<CardModel> GetStasisCards(Player player)
    {
        return TryGetStasisPile(player)?.Cards ?? [];
    }

    public static GuardianPile GetStasisPile(Player player)
    {
        return (GuardianPile)GuardianPile.Stasis.GetPile(player);
    }

    private static GuardianPile? TryGetStasisPile(Player player)
    {
        return CustomPiles.GetCustomPile(player.PlayerCombatState, GuardianPile.Stasis) as GuardianPile;
    }

    public static int GetMaxStasisSlots(Player player)
    {
        return GuardianCombatModel.StasisSlots[player];
    }

    public static void AddMaxStasisSlots(Player player, int value = 1)
    {
        if (value <= 0) return;
        GuardianCombatModel.StasisSlots[player] += value;
        GuardianDisplay.Refresh(player);
    }

    public static void RemoveMaxStasisSlots(Player player, int value = 1)
    {
        if (value <= 0) return;
        GuardianCombatModel.StasisSlots[player] = Math.Max(0, GuardianCombatModel.StasisSlots[player] - value);
        GuardianDisplay.Refresh(player);
    }

    public static bool CanPutIntoStasis(Player player, bool silent = false)
    {
        var pile = GuardianCombatModel.GetOrInitStasis(player);
        if (pile.Cards.Count < GetMaxStasisSlots(player)) return true;
        if (silent || !LocalContext.IsMe(player)) return false;
        ThinkCmd.Play(FullStasisText, player.Creature, 2.0);
        return false;
    }

    public static async Task<bool> PutIntoStasis(CardModel card, PlayerChoiceContext ctx, AbstractModel? source = null,
        bool silent = false)
    {
        if (card.CombatState == null) return false;
        var player = card.Owner;
        var pile = GuardianCombatModel.GetOrInitStasis(player);
        if (pile.Cards.Count >= GetMaxStasisSlots(player))
        {
            if (!silent && LocalContext.IsMe(player))
                ThinkCmd.Play(FullStasisText, player.Creature, 2.0);
            return false;
        }

        source ??= card;
        await GuardianHook.BeforeCardEntersStasis(card.CombatState, ctx, card, source);
        await CardPileCmd.Add(card, pile, clonedBy: source, skipVisuals: silent);
        SetStasisCounter(card);
        card.EnergyCost.AfterCardPlayedCleanup();
        await GuardianHook.AfterCardEntersStasis(card.CombatState, ctx, card, source);
        return true;
    }

    public static int GetStasisCounter(CardModel card)
    {
        return GuardianCombatModel.StasisCounter[card];
    }

    public static void SetStasisCounter(CardModel card)
    {
        GuardianCombatModel.StasisCounter[card] = CalculateStasisCounter(card);
        GuardianDisplay.Refresh(card.Owner);
    }

    private static int CalculateStasisCounter(CardModel card)
    {
        return card is ICustomTickDuration custom ? custom.TickDuration : card.EnergyCost.GetResolved() + 1;
    }

    private static async Task ReturnFromStasis(CardModel card, Player player, PlayerChoiceContext ctx)
    {
        if (card.Keywords.Contains(GuardianKeyword.Volatile))
        {
            await CardCmd.Exhaust(ctx, card);
            return;
        }

        await CardPileCmd.Add(card, PileType.Hand.GetPile(player));
        card.EnergyCost.SetUntilPlayed(0);
    }

    private static async Task TickCard(CardModel card, Player player, PlayerChoiceContext ctx)
    {
        if (GuardianCombatModel.StasisCounter[card] <= 0) return;
        var combatState = player.Creature.CombatState;
        if (combatState == null) return;

        GuardianCombatModel.StasisCounter[card]--;
        GuardianDisplay.RefreshCounters(player);
        if (card is ITickCard tickCard)
            await tickCard.OnTick(ctx);
        await GuardianHook.AfterCardTick(combatState, ctx, card, player);

        if (GuardianCombatModel.StasisCounter[card] == 0)
            await ReturnFromStasis(card, player, ctx);
    }


    public static async Task TickAll(Player player, PlayerChoiceContext ctx)
    {
        foreach (var card in GetStasisCards(player).ToList())
            await TickCard(card, player, ctx);
        GuardianDisplay.Refresh(player);
    }

    // Gems
    public static List<GemModel> GetAllCombatGems(Player player)
    {
        return player.GetAllCards()
            .SelectMany(card => card switch
            {
                IGemCard gem => [gem.GemModel],
                IGemSocketCard gc => gc.Gems,
                _ => []
            })
            .ToList();
    }

    public static async Task PutGemIn(CardModel gem, CardModel card)
    {
        if (card is not IGemSocketCard guardianCard) return;
        if (gem is not IGemCard gemCard) return;
        var gemModel = gemCard.GemModel;
        card.AssertMutable();
        if (!guardianCard.CanAddGem(gemModel)) return;
        guardianCard.AddGem(gemModel);
        await CardPileCmd.RemoveFromDeck(gem, false);
        await Cmd.Wait(0.5f);
        if (LocalContext.IsMe(card.Owner))
            NRun.Instance?.GlobalUi.CardPreviewContainer.AddChildSafely(NCardSmithVfx.Create([card])!);
        await Cmd.Wait(0.5f);
    }

    public static async Task Brace(PlayerChoiceContext ctx, Player player, decimal amount)
    {
        var power = player.Creature.GetPower<ModeShiftPower>();
        if (power == null)
        {
            await PowerCmd.Apply<ModeShiftPower>(ctx, player.Creature, 20, player.Creature, null, true);
            power = player.Creature.GetPower<ModeShiftPower>();
        }
        var modifiedAmount = GuardianHook.ModifyBraceAmount(power!.CombatState, player, amount);
        power.SetAmount((int)(power.Amount - modifiedAmount), true);
        if (power.Amount > 0) return;
        await power.Reset(ctx);
    }

    public static Task Brace(PlayerChoiceContext ctx, CardModel card)
    {
        return Brace(ctx, card.Owner, card.DynamicVars.Brace().IntValue);
    }

    public static async Task Accelerate(PlayerChoiceContext ctx, Player player, int amount = 1,
        AccelerateType accelerateType = AccelerateType.First)
    {
        var cards = GetStasisCards(player).ToList();

        foreach (var card in cards)
        {
            var ticks = accelerateType == AccelerateType.First
                ? Math.Min(amount, GuardianCombatModel.StasisCounter[card])
                : amount;

            for (var i = 0; i < ticks; i++)
                await TickCard(card, player, ctx);

            if (accelerateType != AccelerateType.First) continue;
            amount -= ticks;
            if (amount <= 0) break;
        }

        GuardianDisplay.Refresh(player);
    }

    public static async Task Accelerate(PlayerChoiceContext ctx, CardModel card, Player player, int amount = 1)
    {
        var ticks = Math.Min(amount, GuardianCombatModel.StasisCounter[card]);
        for (var i = 0; i < ticks; i++)
            await TickCard(card, player, ctx);
        GuardianDisplay.Refresh(player);
    }

    public static Task Accelerate(PlayerChoiceContext ctx, CardModel card,
        AccelerateType accelerateType = AccelerateType.First)
    {
        return Accelerate(ctx, card.Owner, card.DynamicVars.Accelerate().IntValue, accelerateType);
    }


    public static async Task Polish(PlayerChoiceContext ctx, CardModel card)
    {
        var amount = card.DynamicVars.Polish().IntValue;
        await Polish(ctx, card, amount);
    }

    public static async Task Polish(PlayerChoiceContext ctx, CardModel card, decimal amount)
    {
        await Polish(ctx, card.Owner.Creature, amount, card);
    }

    public static async Task Polish(PlayerChoiceContext ctx, Creature target, decimal amount, CardModel? cardSource)
    {
        await DecrementPower<WeakPower>(ctx, target, amount, cardSource);
        await DecrementPower<FrailPower>(ctx, target, amount, cardSource);
        await DecrementPower<VulnerablePower>(ctx, target, amount, cardSource);
        var tempPowers = target.Powers.Where(e => e is ITemporaryPower).ToList();
        foreach (var power in tempPowers)
        {
            var temporaryPower = (ITemporaryPower)power;
            var internalTemporaryPower = target.GetPower(temporaryPower.InternallyAppliedPower.Id);
            if (temporaryPower.InternallyAppliedPower.Type != PowerType.Buff || power.Type != PowerType.Buff) continue;

            var hasArtifact = target.GetPower<ArtifactPower>() != null;

            if (hasArtifact)
            {
                // If the target has Artifact, we can ignore the power reduction from removing the temporary power,
                // since the Artifact will prevent itself.
                // To compensate, apply 1 stack Artifact.
                await PowerCmd.Apply<ArtifactPower>(ctx, target, 1, target, cardSource, true);
            }

            else
            {
                // Otherwise, we need to apply the power up manually.
                // Here we apply it *before* removing the temporary power,
                // because if the temporary power, internal power and polish have the same amount, the internal power
                // would be removed because of the temporary power reduction and thus missing.
                if (internalTemporaryPower == null)
                    await PowerCmd.Apply(ctx, temporaryPower.InternallyAppliedPower.ToMutable(), target,
                        amount, target, cardSource, true);
                else
                    await PowerCmd.ModifyAmount(ctx, internalTemporaryPower, amount, target, cardSource, true);
            }

            await PowerCmd.ModifyAmount(ctx, power, -amount, target, cardSource);
        }
    }

    private static async Task DecrementPower<T>(PlayerChoiceContext ctx, Creature ownerCreature, decimal amount = 1,
        CardModel? source = null)
        where T : PowerModel
    {
        var a = ownerCreature.GetPower<T>();
        if (a is not { Amount: > 0 }) return;
        var mod = Math.Min(a.Amount, amount);
        await PowerCmd.ModifyAmount(ctx, a, -mod, ownerCreature, source);
    }
}

public enum AccelerateType
{
    First,
    All
}
