using BaseLib.Utils;
using Downfall.Code.Cards.CardModels;
using Downfall.Code.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Downfall.Code.Cards.Collector.Token;


[Pool(typeof(TokenCardPool))]
public class Trip : CollectorCardModel
{
    public Trip() : base(0, CardType.Skill, CardRarity.Token, TargetType.AnyEnemy)
    {
        WithPower<VulnerablePower>(2);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await MyCommonActions.Apply<VulnerablePower>(this, cardPlay);
    }

    public override TargetType TargetType => IsUpgraded ? TargetType.AllEnemies : TargetType.AnyEnemy;

}