using Collector.CollectorCode.Events;
using Collector.CollectorCode.Extensions;
using Collector.CollectorCode.Powers;
using Downfall.DownfallCode.Abstract;
using Downfall.DownfallCode.Commands;
using Downfall.DownfallCode.Compatibility;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Characters;

namespace Collector.CollectorCode.Core;

public class CollectorCmd
{
    public static async Task<CardModel?> Pyre(PlayerChoiceContext ctx, CardModel card)
    {
        var prefs = new CardSelectorPrefs(CardSelectorPrefs.ExhaustSelectionPrompt, 1, 1);
        var pyred = (await CardSelectCmd.FromHand(ctx, card.Owner, prefs, e => e != card, card)).FirstOrDefault();
        if (pyred == null || card.CombatState == null) return pyred;
        await CardCmdCompatibility.Exhaust(ctx, pyred);
        await CollectorHook.OnPyre(card.CombatState, ctx, card, pyred);
        return pyred;
    }

    public static async Task<Creature> SummonTorchhead(
        PlayerChoiceContext ctx,
        Player summoner,
        int hp,
        AbstractModel? source)
    {
        if (summoner is not { Osty: not null, Torchhead: null }) //User has an Osty already! (But does not have a Torchhead).
            return await DownfallCmd.Summon<TorchheadMonsterModel, TorchheadPower>(ctx, summoner, hp,
                source); //No Osty, summon on Torchhead instead.
        await CreatureCmd.TriggerAnim(summoner.Creature, Necrobinder.GetSummonAnimIfApplicable(summoner.Character), Necrobinder.GetSummonDelayIfApplicable(summoner.Character));
        await OstyCmd.Summon(ctx, summoner, hp, source);
        return await DownfallCmd.Summon<TorchheadMonsterModel, TorchheadPower>(ctx, summoner, hp, source);//No Osty, summon on Torchhead instead.
    }


    public static Task GetReserve(Player player, int amount)
    {
        CardResourceRegistry.Get<CollectorEnergy>()?.Gain(player, amount);
        return Task.CompletedTask;
    }
    
    public static Creature? Torchhead(Player summoner)
    {
        return DownfallCmd.GainPet<TorchheadMonsterModel>(summoner);
    }
}