using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Snecko.SneckoCode.Core;

namespace Snecko.SneckoCode.Cards.Rare;

[Pool(typeof(SneckoCardPool))]
public class TrashToTreasure : SneckoCardModel
{
    public TrashToTreasure() : base(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        WithBlock(9);
        WithKeyword(CardKeyword.Exhaust, UpgradeType.Remove);
        WithTip(CardKeyword.Exhaust);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardBlock(this, cardPlay);
        var prefs = new CardSelectorPrefs(CardSelectorPrefs.ExhaustSelectionPrompt, 1);
        var card = (await CardSelectCmd.FromHand(ctx, Owner, prefs,
            e => e != this, this)).FirstOrDefault();
        if (card == null) return;
        var cost = card.EnergyCost.GetResolved();
        await CardCmd.Exhaust(ctx, card);
        await PlayerCmd.GainEnergy(cost, Owner);
    }
}