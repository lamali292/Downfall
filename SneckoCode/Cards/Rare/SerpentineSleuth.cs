using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Snecko.SneckoCode.Core;
using Snecko.SneckoCode.Powers;

namespace Snecko.SneckoCode.Cards.Rare;

[Pool(typeof(SneckoCardPool))]
public class SerpentineSleuth : SneckoCardModel
{
    public SerpentineSleuth() : base(4, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
        WithGift(new Gift
        {
            Rarity = CardRarity.Rare,
            Type = CardType.Power
        });
        WithPower<SerpentineSleuthPower>(1, 1);
        WithEnergy(1, 1);
        WithKeyword(CardKeyword.Ethereal);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await CommonActions.ApplySelf<SerpentineSleuthPower>(ctx, this);
    }
}