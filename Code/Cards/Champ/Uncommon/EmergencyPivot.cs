using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Abstract.CardModels;
using Downfall.Code.Commands;
using Downfall.Code.Extensions;
using Downfall.Code.Keywords;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Downfall.Code.Cards.Champ.Uncommon;

[Pool(typeof(ChampCardPool))]
public class EmergencyPivot : ChampCardModel
{
    public EmergencyPivot() : base(2, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithKeywords(CardKeyword.Retain);
        WithBlock(10, 4);
        WithTags(DownfallTag.Finisher);
        WithTip(DownfallTip.Finisher);
    }

    protected override bool ShouldGlowRedInternal => Owner.ChampStance().HasFinisher;
    protected override bool IsPlayable => Owner.ChampStance().HasFinisher;

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardBlock(this, cardPlay);
        await ChampCmd.PlayFinisher(ctx, cardPlay);
    }
}