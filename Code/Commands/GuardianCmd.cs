using BaseLib.Patches.Content;
using Downfall.Code.Abstract.CardModels;
using Downfall.Code.Cards.Guardian.Abstract;
using Downfall.Code.Core;
using Downfall.Code.Core.Guardian;
using Downfall.Code.Displays;
using Downfall.Code.Events;
using Downfall.Code.Extensions;
using Downfall.Code.Interfaces;
using Downfall.Code.Piles;
using Downfall.Code.Powers.Guardian;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;

namespace Downfall.Code.Commands;

public static class GuardianCmd
{
    private static readonly LocString FullStasisText = new("combat_messages", "FULL_STASIS_SLOTS");

    // Mode
    public static Task EnterDefensiveMode(Player player) => 
        GuardianModel.SetMode(player, DownfallModelDb.GuardianMode<GuardianDefensiveMode>());
    
    public static Task LeaveDefensiveMode(Player player) => 
        GuardianModel.SetMode(player, DownfallModelDb.GuardianMode<GuardianNormalMode>());
    
    public static Task ChangeMode(Player player) =>
        IsInMode<GuardianNormalMode>(player) ? EnterDefensiveMode(player) : LeaveDefensiveMode(player);

    public static GuardianModeModel GetMode(Player player) => 
        GuardianModel.ActiveMode[player] ?? DownfallModelDb.GuardianMode<GuardianNormalMode>();
    
    public static bool IsInMode<T>(Player player) where T : GuardianModeModel => 
        GuardianModel.ActiveMode[player] is T;

    // Stasis
    public static int GetStasisCount(Player player) => GetStasisPile(player)?.Cards.Count ?? 0;
    public static IReadOnlyList<CardModel> GetStasisCards(Player player) => GetStasisPile(player)?.Cards ?? [];
    public static GuardianPile? GetStasisPile(Player player) => 
        CustomPiles.GetCustomPile(player.PlayerCombatState, GuardianPile.Stasis) as GuardianPile;

    public static int GetMaxStasisSlots(Player player) => GuardianModel.StasisSlots[player];
    public static void AddMaxStasisSlots(Player player, int value = 1)
    {
        if (value <= 0) return;
        GuardianModel.StasisSlots[player] += value;
        GuardianDisplay.Refresh(player);
    }
    public static void RemoveMaxStasisSlots(Player player, int value = 1)
    {
        if (value <= 0) return;
        GuardianModel.StasisSlots[player] = Math.Max(0, GuardianModel.StasisSlots[player] - value);
        GuardianDisplay.Refresh(player);
    }

    public static bool CanPutIntoStasis(Player player)
    {
        var pile = GetStasisPile(player);
        if (pile == null) return false;
        if (pile.Cards.Count < GetMaxStasisSlots(player)) return true;
        if (!LocalContext.IsMe(player)) return false;
        NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(
            NThoughtBubbleVfx.Create(FullStasisText.GetFormattedText(), player.Creature, 2.0));
        return false;
    }

    public static async Task PutIntoStasis(CardModel card, PlayerChoiceContext ctx, AbstractModel? source = null)
    {
        if (card.CombatState == null) return;
        var player = card.Owner;
        if (!CanPutIntoStasis(player)) return;
        card.EnergyCost.AfterCardPlayedCleanup();
        source ??= card;
        await DownfallHook.BeforeCardEntersStasis(card.CombatState, ctx, card, source);
        await CardPileCmd.Add(card, GetStasisPile(player)!, source: source);
        SetStasisCounter(card);
    }

    public static int GetStasisCounter(CardModel card) => GuardianModel.StasisCounter[card];
    public static void SetStasisCounter(CardModel card)
    {
        GuardianModel.StasisCounter[card] = CalculateStasisCounter(card);
        GuardianDisplay.Refresh(card.Owner);
    }
    public static void DecrementStasisCounter(CardModel card)
    {
        if (GuardianModel.StasisCounter[card] <= 0) return;
        GuardianModel.StasisCounter[card]--;
        GuardianDisplay.RefreshCounters(card.Owner);
    }

    private static int CalculateStasisCounter(CardModel card) =>
        card is ICustomTickDuration custom ? custom.TickDuration : card.EnergyCost.GetResolved() + 1;

    // Gems
    public static List<GemModel> GetAllCombatGems(Player player) =>
        player.PlayerCombatState?.AllCards
            .SelectMany(card => card switch
            {
                IGemCard gem => [gem.GemModel],
                GuardianCardModel gc => gc.Gems,
                _ => []
            })
            .ToList() ?? [];

    public static async Task PutGemIn(CardModel gem, CardModel card)
    {
        if (card is not GuardianCardModel guardianCard) return;
        if (gem is not IGemCard gemCard) return;
        if (!guardianCard.CanAddGem(gemCard.GemModel)) return;
        guardianCard.AddGem(gemCard.GemModel);
        await CardPileCmd.RemoveFromDeck(gem, false);
        await Cmd.Wait(0.5f);
        if (LocalContext.IsMe(card.Owner))
            NRun.Instance?.GlobalUi.CardPreviewContainer.AddChildSafely(NCardSmithVfx.Create([card])!);
        await Cmd.Wait(0.5f);
    }

    // Combat
    public static async Task DebuffDown(Creature creature, int amount = 1)
    {
        foreach (var powerModel in creature.Powers
            .Where(p => p.TypeForCurrentAmount == PowerType.Debuff)
            .OrderByDescending(p => p is ITemporaryPower)
            .ToList())
        {
            switch (powerModel.Amount)
            {
                case > 0:
                    await PowerCmd.ModifyAmount(powerModel, -Math.Min(amount, powerModel.Amount), creature, null);
                    break;
                case < 0:
                    await PowerCmd.ModifyAmount(powerModel, Math.Min(amount, Math.Abs(powerModel.Amount)), creature, null);
                    break;
            }
        }
    }

    public static async Task Brace(Player player, int amount)
    {
        var power = player.Creature.GetPower<ModeShiftPower>();
        if (power == null) return;
        if (await PowerCmd.ModifyAmount(power, -amount, player.Creature, null) > 0) return;
        await power.Reset();
    }

    public static Task Brace(CardModel card) => Brace(card.Owner, card.DynamicVars.Brace().IntValue);
    
    public static async Task Accelerate(PlayerChoiceContext ctx, Player player, int amount = 1,
        AccelerateType accelerateType = AccelerateType.First)
    {
        var cards = GetStasisCards(player).ToList(); // oldest first
    
        if (accelerateType == AccelerateType.First)
        {
            // Distribute amount across cards oldest-first
            foreach (var card in cards)
            {
                if (amount <= 0) break;
                var reduce = Math.Min(amount, GuardianModel.StasisCounter[card]);
                amount -= reduce;
                for (var i = 0; i < reduce; i++)
                {
                    GuardianModel.StasisCounter[card]--;
                    GuardianDisplay.RefreshCounters(player);
                    if (card is ITickCard tickCard)
                        await tickCard.OnTick(ctx);
                }

                if (GuardianModel.StasisCounter[card] == 0)
                    await GuardianModel.ReturnFromStasis(card, player, ctx); // see below
            }
        }
        else 
        {
            foreach (var card in cards)
            {
                var reduce = Math.Min(amount, GuardianModel.StasisCounter[card]);

                for (var i = 0; i < reduce; i++)
                {
                    GuardianModel.StasisCounter[card]--;
                    if (card is not ITickCard tickCard) continue;
                    GuardianDisplay.RefreshCounters(player);
                    await tickCard.OnTick(ctx);
                }

                if (GuardianModel.StasisCounter[card] == 0)
                    await GuardianModel.ReturnFromStasis(card, player, ctx);
            }
        }
    
        GuardianDisplay.Refresh(player);
    }

    public static Task Accelerate(PlayerChoiceContext ctx, CardModel card, AccelerateType accelerateType = AccelerateType.First) =>
        Accelerate(ctx, card.Owner, card.DynamicVars.Accelerate().IntValue, accelerateType);

}

public enum AccelerateType
{
    First,
    All
}