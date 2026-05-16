using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Snecko.SneckoCode.Core;

namespace Snecko.SneckoCode.Powers;

public class TrashCanPower : SneckoPowerModel
{
    public override async Task BeforeFlushLate(PlayerChoiceContext ctx, Player player)
    {
        if (player != Owner.Player) return;
        var prefs = new CardSelectorPrefs(CardSelectorPrefs.ExhaustSelectionPrompt, 0, Amount);
        (await CardSelectCmd.FromHand(ctx, Owner.Player, prefs, null, this)).ToList()
            .ForEach(e => CardCmd.Exhaust(ctx, e));
    }
}