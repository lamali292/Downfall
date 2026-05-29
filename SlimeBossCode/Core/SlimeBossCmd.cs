using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using SlimeBoss.SlimeBossCode.Cards.Token;
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


    public static async Task<bool> Absorb(PlayerChoiceContext ctx, CardModel card)
    {
        var a = await SlimeQueue.RemoveLeadingSlime(card.Owner);
        await PowerCmd.Apply<StrengthPower>(ctx, card.Owner.Creature, 1, card.Owner.Creature, card);
        return a;
    }
    
    
    public static async Task<int> AbsorbAll(PlayerChoiceContext ctx, CardModel card)
    {
        var a = await SlimeQueue.RemoveAll(card.Owner);
        await PowerCmd.Apply<StrengthPower>(ctx, card.Owner.Creature, a, card.Owner.Creature, card);
        return a;
    }
    
    private static async Task Command(PlayerChoiceContext ctx, Player player)
    {
        var slime = GetFirstSlime(player);
        if (slime == null) return;
        await slime.Command(ctx);
    }

    public static async Task Command(PlayerChoiceContext ctx, Player player, int amount)
    {
        for (var i = 0; i < amount; i++) await Command(ctx, player);
    }

    public static Task Command(PlayerChoiceContext ctx, CardModel card)
    {
        return Command(ctx, card.Owner, card.DynamicVars["Command"].IntValue);
    }

    
    public static async Task SlurpAll(CardModel card)
    {
        var licks = card.Owner.GetExhaust()
            .Where(e => e.Tags.Contains(SlimeBossTag.Lick))
            .ToList();
        await CardPileCmd.Add(licks, PileType.Hand);
    }
    
    
    public static async Task Slurp(Player player, int amount)
    {
        var licks = player.GetExhaust()
            .Where(e => e.Tags.Contains(SlimeBossTag.Lick))
            .ToList();

        var unburied = licks.Where(e => !e.Keywords.Contains(SlimeBossKeyword.Buried)).ToList();
        var buried = licks.Where(e => e.Keywords.Contains(SlimeBossKeyword.Buried)).ToList();

        var cards = unburied
            .TakeRandom(Math.Min(amount, unburied.Count), player.RunState.Rng.CombatCardSelection)
            .ToList();

        if (cards.Count < amount)
            cards.AddRange(buried.TakeRandom(amount - cards.Count,player.RunState.Rng.CombatCardSelection));

        await CardPileCmd.Add(cards, PileType.Hand);
    }

    
    public static Task Slurp(CardModel card)
     => Slurp(card.Owner, card.DynamicVars["Slurp"].IntValue);


    public static async Task Split<T>(Player player) where T : SlimeModel
    {
        var slimeModel = SlimeBossModelDb.Slime<T>();
        await SlimeQueue.AddSlime(player, slimeModel);
    }

    public static async Task SplitSpecialist(PlayerChoiceContext ctx, Player owner)
    {
        var combatState = owner.Creature.CombatState;
        if (combatState == null) return;
        var slimeCards = SlimeBossModelDb.AllSpecialistSlimes
            .TakeRandom(3, owner.RunState.Rng.CombatCardGeneration)
            .Select(SlimeBossModelDb.GetCardForSlime).Select(e => combatState.CreateCard(e, owner)).ToList();
        var card  = await CardSelectCmd.FromChooseACardScreen(ctx, slimeCards, owner);
        if (card is not ISlimeCard slimeCard) return;
        var slime = slimeCard.SlimeModel;
        await SlimeQueue.AddSlime(owner, slime);
    }
}
