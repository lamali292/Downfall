using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using Snecko.SneckoCode.Core;
using Snecko.SneckoCode.Extensions;
using Snecko.SneckoCode.Interfaces;

namespace Snecko.SneckoCode.Cards.Uncommon;

[Pool(typeof(SneckoCardPool))]
public class MintCondition : SneckoCardModel, IHasOverflowEffect
{
    public MintCondition() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithPower<StrengthPower>(3, 1);
        this.WithOverflow();
    }


    public async Task OverflowEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<StrengthPower>(ctx, this);
    }
}