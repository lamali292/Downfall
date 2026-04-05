using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Cards.CardModels;
using Downfall.Code.Commands;
using Downfall.Code.Powers.Champ;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Downfall.Code.Cards.Champ.Basic;

[Pool(typeof(ChampCardPool))]
public class DefensiveShout : ChampCardModel
{
    public DefensiveShout() : base(0, CardType.Skill, CardRarity.Basic, TargetType.Self)
    {
        WithPower<CounterPower>(3, 3);
        WithIcon<CounterPower>();
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<CounterPower>(this);
        await ChampCmd.EnterDefensiveStance(ctx, Owner);
    }
}