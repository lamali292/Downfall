using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Snecko.SneckoCode.Core;
using Snecko.SneckoCode.Powers;

namespace Snecko.SneckoCode.Cards.Rare;

[Pool(typeof(SneckoCardPool))]
public class ForkedTongue : SneckoCardModel
{

    public ForkedTongue() : base(3, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
        WithCostUpgradeBy(-1);
        this.WithPower<ForkedTonguePower>(1, false);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<ForkedTonguePower>(ctx, this);
    }
    
}