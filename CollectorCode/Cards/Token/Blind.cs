using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Collector.CollectorCode.Cards.Token;

[Pool(typeof(TokenCardPool))]
public class Blind : CollectorCardModel
{
    public Blind() : base(0, CardType.Skill, CardRarity.Token, TargetType.AnyEnemy)
    {
        WithPower<WeakPower>(2);
    }

    public override TargetType TargetType => IsUpgraded ? TargetType.AllEnemies : TargetType.AnyEnemy;

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.Apply<WeakPower>(ctx, this, cardPlay);
    }
}