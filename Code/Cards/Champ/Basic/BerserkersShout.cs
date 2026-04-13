using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Cards.CardModels;
using Downfall.Code.Commands;
using Downfall.Code.Core;
using Downfall.Code.Extensions;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Downfall.Code.Cards.Champ.Basic;

[Pool(typeof(ChampCardPool))]
public class BerserkersShout : ChampCardModel
{
    public BerserkersShout() : base(0, CardType.Skill, CardRarity.Basic, TargetType.Self)
    {
        WithPower<VigorPower>(3);
        WithIcon<VigorPower>();
    }
    
    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await MyCommonActions.ApplySelf<VigorPower>(this);
        await ChampCmd.BerserkerStance(ctx, Owner);
    }


    protected override void OnUpgrade()
    {
        DynamicVars.Power<VigorPower>().UpgradeValueBy(3);
    }
}