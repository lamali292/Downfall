using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Snecko.SneckoCode.Core;

namespace Snecko.SneckoCode.Cards.Uncommon;

[Pool(typeof(SneckoCardPool))]
public class SlitherThrough : SneckoCardModel
{
    public SlitherThrough() : base(2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        WithGift(new Gift
        {
            Rarity = CardRarity.Uncommon
        });
        WithDamage(14, 4);
        WithEnergy(1);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
        Owner.PlayerCombatState?.Hand.Cards
            .Where(e => SneckoCmd.IsOffclass(this, e))
            .ToList().ForEach(e => e.EnergyCost.AddThisTurn(-1));
    }
}