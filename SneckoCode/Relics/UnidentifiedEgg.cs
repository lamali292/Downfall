using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.Helpers.Models;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Runs;
using Snecko.SneckoCode.Core;

namespace Snecko.SneckoCode.Relics;

[Pool(typeof(SneckoRelicPool))]
public class UnidentifiedEgg : SneckoRelicModel
{
    public UnidentifiedEgg() : base(RelicRarity.Rare)
    {
        WithVars(new CardsVar(2));
    }
    
    public override bool HasUponPickupEffect => true;
    public override Task AfterObtained()
    {
        foreach (var card in PileType.Deck.GetPile(Owner)
                     .Cards.Where(c => SneckoCmd.IsOffclass(Owner, c) && c.IsUpgradable)
                     .ToList()
                     .StableShuffle(Owner.RunState.Rng.Niche)
                     .Take(DynamicVars.Cards.IntValue))
            CardCmd.Upgrade(card);
        return Task.CompletedTask;
    }
    
    public override bool TryModifyCardRewardOptionsLate(
        Player player,
        List<CardCreationResult> cardRewards,
        CardCreationOptions options)
    {
        if (player != Owner || options.Flags.HasFlag(CardCreationFlags.NoHookUpgrades))
            return false;
        UpgradeValidCards(cardRewards, e => SneckoCmd.IsOffclass(Owner, e), this);
        return true;
    }

    public override void ModifyMerchantCardCreationResults(
        Player player,
        List<CardCreationResult> cards)
    {
        if (player != Owner)
            return;
        UpgradeValidCards(cards, e => SneckoCmd.IsOffclass(Owner, e),  this);
    }

    public override bool TryModifyCardBeingAddedToDeck(CardModel card, out CardModel? newCard)
    {
        newCard = null;
        if (card.Owner != Owner || !SneckoCmd.IsOffclass(Owner, card) || !card.IsUpgradable)
            return false;
        newCard = Owner.RunState.CloneCard(card);
        CardCmd.Upgrade(newCard, CardPreviewStyle.None);
        return true;
    }

    private static void UpgradeValidCards(
        List<CardCreationResult> cards,
        Func<CardModel, bool>  filter,
        RelicModel eggRelic)
    {
        foreach (var card1 in cards)
        {
            var card2 = card1.Card;
            if (!filter.Invoke(card2) || !card2.IsUpgradable) continue;
            var card3 = eggRelic.Owner.RunState.CloneCard(card2);
            CardCmd.Upgrade(card3);
            card1.ModifyCard(card3, eggRelic);
        }
    }
    
}