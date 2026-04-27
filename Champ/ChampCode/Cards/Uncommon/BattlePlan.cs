using BaseLib.Utils;
using Champ.ChampCode.Core;
using Downfall.DownfallCode.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Champ.ChampCode.Cards.Uncommon;

[Pool(typeof(ChampCardPool))]
public class BattlePlan : ChampCardModel
{
    public BattlePlan() : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithBlock(2, 2);
        WithVar("Scry", 3, 1);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await ChampCmd.EnterDefensiveStance(ctx, Owner);
        await CommonActions.CardBlock(this, cardPlay);
        await ScryCmd.Execute(ctx, Owner, DynamicVars["Scry"].IntValue);
    }
}