using Collector.CollectorCode.Events;
using Downfall.DownfallCode.Commands;
using Downfall.DownfallCode.Compatibility;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

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
        return await DownfallCmd.Summon<TorchheadMonsterModel>(ctx, summoner, hp, source);
    }
}