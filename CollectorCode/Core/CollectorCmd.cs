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
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Characters;

namespace Collector.CollectorCode.Core;

public class CollectorCmd
{
    
    public static Task TorchheadAttack(PlayerChoiceContext ctx, CardModel card)
    {
        var player = card.Owner;
        var damage = card.DynamicVars.CollectorDamage.IntValue;
        return TorchheadAttack(ctx, player, damage);
    }
    
    public static async Task TorchheadAttack(PlayerChoiceContext ctx, Player player, int damage)
    {
        await Cmd.CustomScaledWait(0.1f, 0.3f);
        var shouldTargetAll = CollectorHook.ShouldTorchheadTargetAll(player, out var modifiers);
        await CollectorHook.AfterShouldTorchheadTargetAll(ctx, player, modifiers);
        if (player.Creature.CombatState == null || player.Torchhead?.Monster is not TorchheadMonsterModel torchhead) return;
        var attack = DamageCmd.Attack(damage)
            .FromTorchhead(torchhead)
            .WithHitFx("vfx/vfx_attack_blunt", tmpSfx: "blunt_attack.mp3");
        if (shouldTargetAll)
        {
            await attack.TargetingAllOpponents(player.Creature.CombatState).Execute(ctx);
        }
        else
        {
            var target = player.Creature.CombatState?.HittableEnemies.OrderBy(e => e.CurrentHp).FirstOrDefault();
            if (target == null) return;
            await attack.Targeting(target).Execute(ctx);
        }
    }
    
    
    private static async Task PyreCards(PlayerChoiceContext ctx, CardModel card, IEnumerable<CardModel> pyred)
    {
        if (card.CombatState == null) return;
        foreach (var c in pyred)
        {
            if (CollectorHook.ShouldExhaustPyred(card, c))
            {
                await CardCmdCompatibility.Exhaust(ctx, c);
            }
            await CollectorHook.AfterCardPyred(card.CombatState, ctx, card, c);
            await Cmd.Wait(0.1f);
        }
    }

    public static async Task<CardModel?> Pyre(PlayerChoiceContext ctx, CardModel card)
    {
        var prefs = new CardSelectorPrefs(CardSelectorPrefs.ExhaustSelectionPrompt, 1, 1);
        var pyred = (await CardSelectCmd.FromHand(ctx, card.Owner, prefs, e => e != card, card)).FirstOrDefault();
        if (pyred == null || card.CombatState == null) return pyred;
        await PyreCards(ctx, card, [pyred]);
        return pyred;
    }

    public static async Task<IReadOnlyList<CardModel>> MegaPyre(PlayerChoiceContext ctx, CardModel card)
    {
        if (card.CombatState == null) return [];
        var cards = card.Owner.Hand.ToList();
        await PyreCards(ctx, card, cards);
        return cards;
    }


    
    
    public static Task<Creature> Kindle(
        PlayerChoiceContext ctx,
        AbstractModel source)
    {
        return Kindle(ctx, source.Player, source);
    }

    public static Task<Creature> Kindle(
        PlayerChoiceContext ctx,
        Player summoner,
        AbstractModel source)
    {
        return Kindle(ctx, summoner, source.DynamicVars.Kindle.IntValue, source);
    }
    
    
    public static async Task<Creature> Kindle(
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

    public static Task GetReserve(CardModel card)
    {
        return GetReserve(card.Owner, card.DynamicVars.Reserve.IntValue );
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