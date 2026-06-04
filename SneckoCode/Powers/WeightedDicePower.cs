using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using Snecko.SneckoCode.Core;
using Snecko.SneckoCode.CustomEnums;
using Snecko.SneckoCode.Events;
using Snecko.SneckoCode.History;

namespace Snecko.SneckoCode.Powers;

public class WeightedDicePower : SneckoPowerModel, IAfterCardMuddled
{
    public WeightedDicePower()
    {
        WithTip(SneckoKeywords.Muddle);
    }
    public Task AfterCardMuddled(PlayerChoiceContext ctx, CardModel card, AbstractModel? source)
    {
        if (card.Owner.Creature != Owner || CardsMuddled >= Amount) return Task.CompletedTask;
        Flash();
        return CardPileCmd.Draw(ctx, card.Owner);
    }
    
    private int CardsMuddled => CombatManager.Instance.History.Entries.OfType<MuddleEntry>().Count(e => e.HappenedThisTurn(CombatState) && e.Actor == Owner);
}