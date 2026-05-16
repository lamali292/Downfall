using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Snecko.SneckoCode.Core;
using Snecko.SneckoCode.Powers;

namespace Snecko.SneckoCode.Cards.Rare;

[Pool(typeof(SneckoCardPool))]
public class SnakeEyes : SneckoCardModel
{
    public SnakeEyes() : base(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        WithGift(new Gift
        {
            Rarity = CardRarity.Rare,
            Type = CardType.Skill
        });
        WithPower<SnakeEyesPower>(1, 1);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await CommonActions.ApplySelf<SnakeEyesPower>(ctx, this);
    }
}