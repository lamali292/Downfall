using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Snecko.SneckoCode.Core;

namespace Snecko.SneckoCode.Cards.Ancient;

[Pool(typeof(SneckoCardPool))]
public class AncientTwo : SneckoCardModel
{
    public AncientTwo() : base(0, CardType.Skill, CardRarity.Ancient, TargetType.None)
    {
        WithKeywords(CardKeyword.Ethereal, CardKeyword.Exhaust);
        WithCards(1);
    }
    
    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.Draw(this, ctx);
    }
}