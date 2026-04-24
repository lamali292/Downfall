using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Abstract.CardModels;
using Downfall.Code.Commands;
using Downfall.Code.Powers.Champ;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Downfall.Code.Cards.Champ.Uncommon;

[Pool(typeof(ChampCardPool))]
public class FancyFootwork : ChampCardModel
{
    public FancyFootwork() : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithPower<FancyFootworkPower>(10, 5);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await ChampCmd.EnterDifferentStance(ctx, Owner);
        await CommonActions.ApplySelf<FancyFootworkPower>(ctx, this);
    }
}