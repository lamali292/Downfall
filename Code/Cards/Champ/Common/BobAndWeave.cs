using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Cards.CardModels;
using Downfall.Code.Commands;
using Downfall.Code.Extensions;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Downfall.Code.Cards.Champ.Common;

[Pool(typeof(ChampCardPool))]
public class BobAndWeave : ChampCardModel
{
    public BobAndWeave() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        WithBlock(4);
        WithPower<VigorPower>(4);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardBlock(this, cardPlay);
        await CommonActions.ApplySelf<VigorPower>(this);
        await ChampCmd.EnterBerserkerStance(ctx, Owner);
    }


    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(2);
        DynamicVars.Power<VigorPower>().UpgradeValueBy(1);
    }
}