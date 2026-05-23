using Downfall.DownfallCode.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using SlimeBoss.SlimeBossCode.CustomEnums;
using SlimeBoss.SlimeBossCode.Slimes;

namespace SlimeBoss.SlimeBossCode.Core;

public static class SlimeBossCmd
{
    private static IEnumerable<SlimeModel> GetSlimes(Player player)
    {
        return player.Creature.Pets.Select(e => e.Monster).OfType<SlimeModel>();
    }

    private static SlimeModel? GetFirstSlime(Player player)
    {
        return GetSlimes(player).LastOrDefault();
    }


    private static async Task Command(PlayerChoiceContext ctx, Player player)
    {
        var slime = GetFirstSlime(player);
        if (slime == null) return;
        await slime.Command(ctx);
    }

    private static async Task Command(PlayerChoiceContext ctx, Player player, int amount)
    {
        for (var i = 0; i < amount; i++) await Command(ctx, player);
    }

    public static Task Command(PlayerChoiceContext ctx, CardModel card)
    {
        return Command(ctx, card.Owner, card.DynamicVars["Command"].IntValue);
    }

    public static async Task Slurp(CardModel card)
    {
        var amount = card.DynamicVars["Slurp"].IntValue;
        var licks = card.Owner.GetExhaust()
            .Where(e => e.Tags.Contains(SlimeBossTag.Lick))
            .ToList();

        var unburied = licks.Where(e => !e.Keywords.Contains(SlimeBossKeyword.Buried)).ToList();
        var buried = licks.Where(e => e.Keywords.Contains(SlimeBossKeyword.Buried)).ToList();

        var cards = unburied
            .TakeRandom(Math.Min(amount, unburied.Count), card.Owner.RunState.Rng.CombatCardSelection)
            .ToList();

        if (cards.Count < amount)
            cards.AddRange(buried.TakeRandom(amount - cards.Count, card.Owner.RunState.Rng.CombatCardSelection));

        await CardPileCmd.Add(cards, PileType.Hand);
    }
}