using BaseLib.Patches.Content;
using Collector.CollectorCode.Events;
using Collector.CollectorCode.Piles;
using Downfall.DownfallCode.Commands;
using Godot;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.Rooms;

namespace Collector.CollectorCode.Core;

public class CollectorCmd
{
    public static async Task<CardModel?> Pyre(PlayerChoiceContext ctx, CardModel card)
    {
        var prefs = new CardSelectorPrefs(CardSelectorPrefs.ExhaustSelectionPrompt, 1, 1);
        var pyred = (await CardSelectCmd.FromHand(ctx, card.Owner, prefs, e => e != card, card)).FirstOrDefault();
        if (pyred == null || card.CombatState == null) return pyred;
        await CardCmd.Exhaust(ctx, pyred);
        await CollectorHook.OnPyre(card.CombatState, ctx, card, pyred);
        return pyred;
    }


    public static async Task<CardPileAddResult> DrawCollected(PlayerChoiceContext ctx, Player player)
    {
        CollectorMainFile.Logger.Info($"DrawCollected: PileType = {CollectorPile.Collected}");
        CollectorMainFile.Logger.Info(
            $"Is registered: {CustomPiles.CustomPileProviders.ContainsKey(CollectorPile.Collected)}");
        if (player.Creature.CombatState == null) return default;
        return await DownfallCardCmd.DrawFromCustomPile(ctx, player, CollectorPile.Collected);
    }

    public static async Task<IReadOnlyList<CardPileAddResult>> DrawCollected(PlayerChoiceContext ctx, Player player,
        int amount)
    {
        if (player.Creature.CombatState == null) return [];
        return await DownfallCardCmd.DrawFromCustomPile(ctx, player, CollectorPile.Collected, amount);
    }


    public static async Task<Creature> SummonTorchhead(
        PlayerChoiceContext ctx,
        Player summoner,
        int hp,
        AbstractModel? source)
    {
        return await DownfallCmd.Summon<TorchheadMonsterModel>(ctx, summoner, hp, source);
    }


    public static Creature? Torchhead(Player summoner)
    {
        return DownfallCmd.GainPet<TorchheadMonsterModel>(summoner);
    }



}