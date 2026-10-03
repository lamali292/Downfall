using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using Snecko.SneckoCode.Core;

namespace Snecko.SneckoCode.Cards.Uncommon;

[Pool(typeof(SneckoCardPool))]
public class MintCondition : SneckoCardModel
{
    public MintCondition() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithPower<StrengthPower>(3, 1);
        WithOverflow();
    }

    protected override Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        return OverflowCmd.Overflow(cardPlay, 
            () => CommonActions.ApplySelf<StrengthPower>(ctx, this)
            );
    }
}