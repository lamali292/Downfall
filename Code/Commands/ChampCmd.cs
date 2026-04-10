using BaseLib.Utils;
using Downfall.Code.Cards.CardModels;
using Downfall.Code.Cards.Champ.Basic;
using Downfall.Code.Core.Champ;
using Downfall.Code.Events;
using Downfall.Code.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace Downfall.Code.Commands;

public class ChampCmd
{
    public static async Task EnterBerserkerStance(PlayerChoiceContext ctx, Player player, bool force = false)
    {
        if (!force && player.ChampStance() is UltimateChampStance stance)
        {
            stance.ResetCharges();
        }
        else
        {
            await ChampModel.SetStance<BerserkerChampStance>(ctx, player);
        }
    }

    public static async Task EnterDefensiveStance(PlayerChoiceContext ctx, Player player, bool force = false)
    {
        if (!force && player.ChampStance() is UltimateChampStance stance)
        {
            stance.ResetCharges();
        }
        else
        {
            await ChampModel.SetStance<DefensiveChampStance>(ctx, player);
        }
        
        
    }

    public static async Task EnterUltimateStance(PlayerChoiceContext ctx, Player player)
    {
        await ChampModel.SetStance<UltimateChampStance>(ctx, player);
    }

    public static async Task EnterGladiatorStance(PlayerChoiceContext ctx, Player player)
    {
        await ChampModel.SetStance<GladiatorChampStance>(ctx, player);
    }

    public static async Task EnterStance<T>(PlayerChoiceContext ctx, Player player) where T : ChampStanceModel
    {
        await ChampModel.SetStance<T>(ctx, player);
    }

    public static async Task EnterDifferentStance(PlayerChoiceContext ctx, Player owner)
    {
        var stance = owner.ChampStance();
        switch (stance)
        {
            case BerserkerChampStance:
                await EnterDefensiveStance(ctx, owner);
                break;
            case DefensiveChampStance:
                await EnterBerserkerStance(ctx, owner);
                break;
            default:
            {
                var rng = owner.Creature.CombatState!.RunState.Rng.CombatCardSelection;
                if (rng.NextBool())
                    await EnterDefensiveStance(ctx, owner);
                else
                    await EnterBerserkerStance(ctx, owner);
                break;
            }
        }
    }

    public static async Task ClearStance(PlayerChoiceContext ctx, Player player)
    {
        await ChampModel.SetStance<NoChampStance>(ctx, player);
    }

    public static async Task PlayFinisher(PlayerChoiceContext ctx, CardPlay cardPlay, bool skipClear = false,
        int repeat = 1)
    {
        var player = cardPlay.Card.Owner;
        var m = player.ChampStance();
        if (!m.HasFinisher) return;

        for (var i = 0; i < repeat; i++)
        {
            await m.Finisher(ctx);
            await DownfallHook.OnFinisher(player.Creature.CombatState!, ctx, cardPlay);
        }

        if (skipClear || m is UltimateChampStance) return;
        await ClearStance(ctx, player);
    }


    public static async Task SelectStanceToEnter(PlayerChoiceContext ctx, Player owner)
    {
        var choices = new List<CardModel>
        {
            owner.Creature.CombatState!.CreateCard<StanceDanceBerserker>(owner),
            owner.Creature.CombatState!.CreateCard<StanceDanceDefensive>(owner)
        };

        var chosen = await CardSelectCmd.FromChooseACardScreen(ctx, choices, owner);
        if (chosen == null) return;
        switch (chosen)
        {
            case StanceDanceBerserker:
                await EnterBerserkerStance(ctx, owner);
                break;
            case StanceDanceDefensive:
                await EnterDefensiveStance(ctx, owner);
                break;
        }
    }
}

// Todo: rename or make dynamic title
[Pool(typeof(TokenCardPool))]
public class StanceDanceBerserker() : ChampCardModel(-1, CardType.Skill, CardRarity.Token, TargetType.Self)
{
    public override string CustomPortraitPath => ModelDb.Card<BerserkersShout>().CustomPortraitPath;
}

[Pool(typeof(TokenCardPool))]
public class StanceDanceDefensive() : ChampCardModel(-1, CardType.Skill, CardRarity.Token, TargetType.Self)
{
    public override string CustomPortraitPath => ModelDb.Card<DefensiveShout>().CustomPortraitPath;
}