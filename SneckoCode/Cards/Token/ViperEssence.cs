using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Snecko.SneckoCode.Cards.Token;

[Pool(typeof(TokenCardPool))]
public class ViperEssence : SneckoCardModel
{
    public ViperEssence() : base(0, CardType.Power, CardRarity.Token, TargetType.Self)
    {
        WithKeyword(CardKeyword.Ethereal);
        WithPower<StrengthPower>(1, 1);
    }

    protected override Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        return CommonActions.ApplySelf<StrengthPower>(ctx, this);
    }
}