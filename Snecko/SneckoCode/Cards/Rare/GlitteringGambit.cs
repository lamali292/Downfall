using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Snecko.SneckoCode.Core;

namespace Snecko.SneckoCode.Cards.Rare;

[Pool(typeof(SneckoCardPool))]
public class GlitteringGambit : SneckoCardModel
{
    public GlitteringGambit() : base(1, CardType.Power, CardRarity.Rare, TargetType.None)
    {
        WithGift(new Gift
        {
            Rarity = CardRarity.Rare,
            IsUpgraded = true,
            Gold = 150
        });
    }

    // TODO: Implement
    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
    }
}