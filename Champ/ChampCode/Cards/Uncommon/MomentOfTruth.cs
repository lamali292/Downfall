using BaseLib.Utils;
using Champ.ChampCode.Core;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Champ.ChampCode.Cards.Uncommon;

[Pool(typeof(ChampCardPool))]
public class MomentOfTruth : ChampCardModel
{
    public MomentOfTruth() : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithCards(1, 1);
        WithFinisher();
        WithKeywords(CardKeyword.Retain);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.Draw(this, ctx);
        await ChampCmd.PlayFinisher(ctx, cardPlay);
    }
}