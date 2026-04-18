using BaseLib.Extensions;
using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Abstract.CardModels;
using Downfall.Code.Cards.CardModels;
using Downfall.Code.Commands;
using Downfall.Code.Extensions;
using Downfall.Code.Keywords;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Downfall.Code.Cards.Champ.Uncommon;

[Pool(typeof(ChampCardPool))]
public class RopeADope : ChampCardModel
{
    public RopeADope() : base(2, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithTags(DownfallTag.Finisher);
        WithTip(DownfallTip.Finisher);
        WithBlock(8, 2);
        WithPower<EnergyNextTurnPower>(1, 1);
        WithEnergy(1, 1);
        WithPower<DrawCardsNextTurnPower>(2);
        WithCards(1);
    }

    protected override bool ShouldGlowRedInternal => Owner.ChampStance().HasFinisher;
    protected override bool IsPlayable => Owner.ChampStance().HasFinisher;

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardBlock(this, cardPlay);
        await CommonActions.ApplySelf<EnergyNextTurnPower>(this);
        await CommonActions.ApplySelf<DrawCardsNextTurnPower>(this);
        await ChampCmd.PlayFinisher(ctx, cardPlay);
    }
}