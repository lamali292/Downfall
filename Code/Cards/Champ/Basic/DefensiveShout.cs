using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Cards.CardModels;
using Downfall.Code.Commands;
using Downfall.Code.Extensions;
using Downfall.Code.Powers.Champ;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;

namespace Downfall.Code.Cards.Champ.Basic;

[Pool(typeof(ChampCardPool))]
public class DefensiveShout : ChampCardModel
{
    public DefensiveShout() : base(0, CardType.Skill, CardRarity.Basic, TargetType.Self)
    {
        WithPower<CounterPower>(4);
        WithIcon<CounterPower>();
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await MyCommonActions.ApplySelf<CounterPower>(this);
        await ChampCmd.EnterDefensiveStance(ctx, Owner);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Power<CounterPower>().UpgradeValueBy(4);
    }
}