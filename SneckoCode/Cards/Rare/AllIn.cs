using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Snecko.SneckoCode.Core;
using Snecko.SneckoCode.CustomEnums;
using Snecko.SneckoCode.Powers;

namespace Snecko.SneckoCode.Cards.Rare;

[Pool(typeof(SneckoCardPool))]
public class AllIn : SneckoCardModel
{
    public AllIn() : base(0, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
    {
        WithDamage(6, 3);
        WithTip(SneckoKeywords.Muddle);
    }

    protected override bool HasEnergyCostX => true;


    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var x = ResolveEnergyXValue();
        await CommonActions.CardAttack(this, cardPlay, x).Execute(ctx);
        if (IsUpgraded) x++;
        await CommonActions.ApplySelf<GamblePower>(ctx, this, x);

    }
}