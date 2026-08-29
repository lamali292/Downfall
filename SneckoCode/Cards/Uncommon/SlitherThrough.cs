using BaseLib.Utils;
using Downfall.DownfallCode.Commands;
using Downfall.DownfallCode.CustomEnums;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Snecko.SneckoCode.Core;
using Snecko.SneckoCode.Interfaces;

namespace Snecko.SneckoCode.Cards.Uncommon;

[Pool(typeof(SneckoCardPool))]
public class SlitherThrough : SneckoCardModel, IHasGift
{
    public SlitherThrough() : base(2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        WithGift(new Gift
        {
            Rarity = CardRarity.Uncommon
        });
        WithTip(DownfallTip.Offclass);
        WithDamage(14, 4);
        WithEnergy(1);
    }

    public Gift? Gift { get; set; }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
        Owner.Hand
            .Where(DownfallCmd.IsOffclass)
            .ToList().ForEach(e => e.EnergyCost.AddThisTurn(-1));
    }
}