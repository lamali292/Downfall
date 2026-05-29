using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Snecko.SneckoCode.Core;
using Snecko.SneckoCode.Extensions;
using Snecko.SneckoCode.Interfaces;

namespace Snecko.SneckoCode.Cards.Uncommon;

[Pool(typeof(SneckoCardPool))]
public class SlitherThrough : SneckoCardModel, IHasGift
{
    public SlitherThrough() : base(2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        this.WithGift(new Gift
        {
            Rarity = CardRarity.Uncommon
        });
        WithDamage(14, 4);
        WithEnergy(1);
    }

    public Gift? Gift { get; set; }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
        Owner.GetHand()
            .Where(e => SneckoCmd.IsOffclass(this, e))
            .ToList().ForEach(e => e.EnergyCost.AddThisTurn(-1));
    }
}