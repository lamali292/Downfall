using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Snecko.SneckoCode.Cards.Token;

[Pool(typeof(TokenCardPool))]
public class HoleUp : SneckoCardModel
{
    public HoleUp() : base(1, CardType.Skill, CardRarity.Token, TargetType.Self)
    {
        WithPower<WeakPower>(2);
        WithBlock(12, 4);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardBlock(this, cardPlay);
        await CommonActions.ApplySelf<WeakPower>(ctx, this);
    }
}