using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Snecko.SneckoCode.Core;

namespace Snecko.SneckoCode.Relics;

[Pool(typeof(SneckoRelicPool))]
public class RareBoosterBox() : SneckoRelicModel(RelicRarity.Shop)
{
      
    public override bool HasUponPickupEffect => true;
    public override async Task AfterObtained()
    {
        var a = SneckoModel.GetRewardSneckoCards(Owner).Where(c => c.Rarity == CardRarity.Rare).TakeRandom(1,
            Owner.RunState.Rng.CombatCardSelection).FirstOrDefault();
        if (a == null) return;
        var card = a.ToMutable();
        Owner.RunState.AddCard(card, Owner);
        var result = await CardPileCmd.Add(card, PileType.Deck);
        CardCmd.PreviewCardPileAdd(result, 0.1f);
    }
}