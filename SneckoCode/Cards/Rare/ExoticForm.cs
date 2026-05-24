using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Snecko.SneckoCode.Core;
using Snecko.SneckoCode.Powers;

namespace Snecko.SneckoCode.Cards.Rare;

[Pool(typeof(SneckoCardPool))]
public class ExoticForm : SneckoCardModel
{
    public ExoticForm() : base(3, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
        WithGift(new Gift
        {
            Rarity = CardRarity.Rare
        });
        WithKeyword(CardKeyword.Ethereal, UpgradeType.Remove);
        WithPower<ExoticFormPower>(1);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await CommonActions.ApplySelf<ExoticFormPower>(ctx, this);
    }
}