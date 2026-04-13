using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Cards.CardModels;
using Downfall.Code.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Downfall.Code.Cards.Champ.Uncommon;

[Pool(typeof(ChampCardPool))]
public class BattlePlan : ChampCardModel
{
    public BattlePlan() : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithBlock(2);
        WithVar("Scry", 3);
    }
    
    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await ChampCmd.DefensiveStance(ctx, Owner);
        await CommonActions.CardBlock(this, cardPlay);
        await ScryCmd.Execute(ctx, Owner, DynamicVars["Scry"].IntValue);
    }


    protected override void OnUpgrade()  
    {
        DynamicVars.Block.UpgradeValueBy(2);
        DynamicVars["Scry"].UpgradeValueBy(1);
    }
}