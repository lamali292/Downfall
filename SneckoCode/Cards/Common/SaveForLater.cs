using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Snecko.SneckoCode.Core;
using Snecko.SneckoCode.Powers;

namespace Snecko.SneckoCode.Cards.Common;

[Pool(typeof(SneckoCardPool))]
public class SaveForLater : SneckoCardModel
{
    public SaveForLater() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
        WithPower<SaveForLaterPower>(1, 1);
        WithDamage(8, 3);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
        await CommonActions.ApplySelf<SaveForLaterPower>(ctx, this);
    }
}