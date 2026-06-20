using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Snecko.SneckoCode.Core;
using Snecko.SneckoCode.CustomEnums;

namespace Snecko.SneckoCode.Cards.Rare;

[Pool(typeof(SneckoCardPool))]
public class AllIn : SneckoCardModel
{
    protected override bool HasEnergyCostX => true;

    public AllIn() : base(0, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
    {
        WithDamage(8, 2);
        WithKeyword(CardKeyword.Exhaust);
        WithTip(SneckoKeywords.Muddle);
    }
    

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var x = ResolveEnergyXValue();
        await CommonActions.CardAttack(this, cardPlay, x).Execute(ctx);
        await SneckoCmd.Muddle(ctx, Owner.GetHand(), this);
    }
}