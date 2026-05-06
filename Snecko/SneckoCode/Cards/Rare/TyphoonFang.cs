using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Snecko.SneckoCode.Core;
using Snecko.SneckoCode.Powers;

namespace Snecko.SneckoCode.Cards.Rare;

[Pool(typeof(SneckoCardPool))]
public class TyphoonFang : SneckoCardModel
{
    public TyphoonFang() : base(1, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
    {
        WithDamage(12, 4);
        WithOverflow();
        WithPower<TyphoonFangPower>(1);
    }
    
    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
    }

    protected override async Task OverflowEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (cardPlay.IsAutoPlay) return;
        var power = await CommonActions.ApplySelf<TyphoonFangPower>(ctx, this);
        power?.SetCard(this);
    }
}