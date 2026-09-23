using BaseLib.Utils;
using Collector.CollectorCode.Core;
using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Runs;

namespace Collector.CollectorCode.Relics;

[Pool(typeof(CollectorRelicPool))]
public class TheContract : CollectorRelicModel
{
    private bool active;
    public TheContract() : base(RelicRarity.Uncommon)
    {
        WithCards(5);
        active = true;
    }
    
    public override async Task AfterObtained()
    {
        CardCreationOptions options = new CardCreationOptions([ModelDb.CardPool<CollectibleCardPool>()], CardCreationSource.Other, CardRarityOddsType.RegularEncounter);
        var reward = new CardReward(options, 5, Owner);
        await RewardsCmd.OfferCustom(Owner, [reward]);
    }
    public override bool TryModifyCardRewardOptionsLate(Player player, List<CardCreationResult> cardRewards, CardCreationOptions options)
    {
        if (active)
        {
            if (options.Flags.HasFlag(CardCreationFlags.NoHookUpgrades))
            {
                return false;
            }
            UpgradeValidCards(cardRewards, _ => true);
            active = false;
            return true;
        }
        return false;
    }

    private static void UpgradeValidCards(IEnumerable<CardCreationResult> cards, Predicate<CardModel> filter)
    {
        foreach (CardCreationResult cardCreationResult in cards.Where(c => c.Card.IsUpgradable && filter(c.Card)))
        {
            CardModel card = cardCreationResult.Card.Owner.RunState.CloneCard(cardCreationResult.Card);
            CardCmd.Upgrade(card);
            cardCreationResult.ModifyCard(card);
        }
    }
}