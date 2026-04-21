using BaseLib.Patches.Content;
using BaseLib.Utils;
using Downfall.Code.Abstract.CardModels;
using Downfall.Code.Cards.Guardian.Abstract;
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

public class GuardianCmd
{
    private static readonly LocString FullStasisText = new("combat_messages", "FULL_STASIS_SLOTS");


    private static readonly SpireField<CardModel, int> StasisCounter = new(_ => 0);

    public static async Task LeaveDefensiveMode(Player player)
    {
        await GuardianModel.SetMode<GuardianNormalMode>(player);
    }

    public static async Task EnterDefensiveMode(Player player)
    {
        await GuardianModel.SetMode<GuardianDefensiveMode>(player);
    }

    public static async Task ChangeMode(Player player)
    {
        if (GuardianModel.IsInMode<GuardianNormalMode>(player))
            await EnterDefensiveMode(player);
        else
            await LeaveDefensiveMode(player);
    }

    public static async Task DebuffDown(Creature creature, int amount = 1)
    {
        // TODO: make it like sts1 with temporary powers
        var debuffs = creature.Powers
            .Where(p => p.TypeForCurrentAmount == PowerType.Debuff)
            .OrderByDescending(p => p is ITemporaryPower)
            .ToList();

        foreach (var powerModel in debuffs)
            switch (powerModel.Amount)
            {
                case > 0:
                {
                    var reduction = Math.Min(amount, powerModel.Amount);
                    await PowerCmd.ModifyAmount(powerModel, -reduction, creature, null);
                    break;
                }
                case < 0:
                {
                    var increase = Math.Min(amount, Math.Abs(powerModel.Amount));
                    await PowerCmd.ModifyAmount(powerModel, increase, creature, null);
                    break;
                }
            }
    }

    public static async Task Brace(Player player, int amount)
    {
        var power = player.Creature.GetPower<ModeShiftPower>();
        if (power == null) return;
        var a = await PowerCmd.ModifyAmount(power, -amount, player.Creature, null);
        if (a > 0) return;
        await power.Reset();
    }

    public static async Task Brace(CardModel card)
    {
        await Brace(card.Owner, card.DynamicVars.Brace().IntValue);
    }


    public static async Task PutGemIn(CardModel gem, CardModel card)
    {
        if (card is not GuardianCardModel guardianCard) return;
        if (gem is not IGemCard gemCard) return;
        if (!guardianCard.CanAddGem(gemCard.GemModel)) return;

        guardianCard.AddGem(gemCard.GemModel);
        await CardPileCmd.RemoveFromDeck(gem, false);
        await Cmd.Wait(0.5f);
        if (LocalContext.IsMe(card.Owner))
            NRun.Instance?.GlobalUi.CardPreviewContainer.AddChildSafely(NCardSmithVfx.Create([
                card
            ])!);
        await Cmd.Wait(0.5f);
    }


    public static int GetStasisCount(Player creature)
    {
        return GetStasisPile(creature)?.Cards.Count ?? 0;
    }

    public static IReadOnlyList<CardModel> GetStasisCards(Player creature)
    {
        return GetStasisPile(creature)?.Cards ?? [];
    }

    public static GuardianPile? GetStasisPile(Player creature)
    {
        return CustomPiles.GetCustomPile(creature.PlayerCombatState, GuardianPile.Stasis) as
            GuardianPile;
    }

    public static int GetMax(Player trackedPlayer)
    {
        return GuardianModel.GetMaxStasisSlots(trackedPlayer);
    }

    public static bool CanPutIntoStasis(Player player)
    {
        var pile = GetStasisPile(player);
        if (pile == null) return false;
        var isMe = LocalContext.IsMe(player);
        if (pile.Cards.Count < GetMax(player)) return true;
        if (!isMe) return false;
        var text = FullStasisText.GetFormattedText();
        var child = NThoughtBubbleVfx.Create(text, player.Creature, 2.0);
        NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(child);
        return false;
    }

    public static async Task PutIntoStasis(CardModel card,
        PlayerChoiceContext ctx,
        AbstractModel? source = null)
    {
        if (card.CombatState == null) return;
        var player = card.Owner;
        var pile = GetStasisPile(player);
        if (pile == null) return;
        if (!CanPutIntoStasis(player)) return;
        card.EnergyCost.AfterCardPlayedCleanup();
        source ??= card;
        await DownfallHook.BeforeCardEntersStasis(card.CombatState, ctx, card, source);
        await CardPileCmd.Add(card, pile, source: source);
        SetStasisCounter(card);

        //if (isMe) await GuardianDisplay.AnimateCardToStasis(card, pile, player);
        /*
        if (card.Pile?.Type == PileType.Hand)
        {

            var hand = NCombatRoom.Instance?.Ui.Hand;
            hand?.Remove(card);

        }
        else if (card.Pile?.Type == PileType.Play)
        {
            card.RemoveFromCurrentPile();
        }


    */
    }

    private static int CalculateStasisCounter(CardModel card)
    {
        if (card is ICustomTickDuration customTickDuration) return customTickDuration.TickDuration;
        var energyCost = card.EnergyCost.GetResolved();
        var stasisCounter = energyCost + 1;
        return stasisCounter;
    }

    public static async Task Accelerate(Player owner, int amout = 1,
        AccelerateType accelerateType = AccelerateType.First)
    {
        throw new NotImplementedException();
    }

    public static async Task Accelerate(CardModel card, AccelerateType accelerateType = AccelerateType.First)
    {
        await Accelerate(card.Owner, card.DynamicVars.Accelerate().IntValue, accelerateType);
    }

    public static int GetStasisCounter(CardModel card)
    {
        return StasisCounter[card];
    }

    public static void SetStasisCounter(CardModel card)
    {
        StasisCounter[card] = CalculateStasisCounter(card);
        GuardianDisplay.Refresh(card.Owner);
    }

    public static void DecrementStasisCounter(CardModel card)
    {
        var current = StasisCounter[card];
        if (current <= 0) return;
        StasisCounter[card] = current - 1;
        GuardianDisplay.RefreshCounters(card.Owner);
    }
}

public enum AccelerateType
{
    First,
    All
}