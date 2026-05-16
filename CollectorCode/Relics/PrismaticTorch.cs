using BaseLib.Utils;
using Collector.CollectorCode.Cards.Token;
using Collector.CollectorCode.Core;
using Downfall.DownfallCode.Abstract;
using Downfall.DownfallCode.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Collector.CollectorCode.Relics;

[Pool(typeof(CollectorRelicPool))]
public class PrismaticTorch : CollectorRelicModel
{
    public PrismaticTorch() : base(RelicRarity.Starter)
    {
        WithTip(typeof(Ember));
    }

    public override async Task BeforeHandDraw(
        Player player,
        PlayerChoiceContext ctx,
        ICombatState combatState)
    {
        if (player != Owner || combatState.RoundNumber > 1) return;
        await DownfallCardCmd.GiveCard<Ember>(Owner, PileType.Hand);
        CardResourceRegistry.Get<CollectorEnergy>()?.Gain(Owner, 1);
        Flash();
    }

    public override Task AfterCardExhausted(PlayerChoiceContext choiceContext, CardModel card, bool causedByEthereal)
    {
        var state = Owner.Creature.CombatState;
        if (card.Owner != Owner ||
            card is not Ember ||
            CombatManager.Instance.History.Entries.OfType<CardExhaustedEntry>().Any(e =>
                e.HappenedThisTurn(state) && e.Card is Ember && e.Card != card)
           ) return Task.CompletedTask;
        CardResourceRegistry.Get<CollectorEnergy>()?.Gain(Owner, 1);
        Flash();
        return Task.CompletedTask;
    }
}