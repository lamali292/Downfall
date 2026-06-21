using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Snecko.SneckoCode.Core;
using Snecko.SneckoCode.Powers;

namespace Snecko.SneckoCode.Cards.Uncommon;

[Pool(typeof(SneckoCardPool))]
public class UnendingSupply : SneckoCardModel
{
    public UnendingSupply() : base(2, CardType.Power, CardRarity.Uncommon, TargetType.Self)
    {
        WithPower<UnendingSupplyPower>(1);
        WithKeyword(CardKeyword.Innate, UpgradeType.Add);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<UnendingSupplyPower>(ctx, this);
    }
}