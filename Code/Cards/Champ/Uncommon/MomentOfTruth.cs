using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Cards.CardModels;
using Downfall.Code.Commands;
using Downfall.Code.Extensions;
using Downfall.Code.Keywords;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Downfall.Code.Cards.Champ.Uncommon;

[Pool(typeof(ChampCardPool))]
public class MomentOfTruth : ChampCardModel
{
    public MomentOfTruth() : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithCards(1, 1);
        WithTags(DownfallTag.Finisher);
        WithTip(DownfallKeyword.Finisher);
        WithKeywords(CardKeyword.Retain);
    }


    protected override bool ShouldGlowRedInternal => Owner.ChampStance().HasFinisher;
    protected override bool IsPlayable => Owner.ChampStance().HasFinisher;

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.Draw(this, ctx);
        await ChampCmd.PlayFinisher(ctx, cardPlay);
    }
}