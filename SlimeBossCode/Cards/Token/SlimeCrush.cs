using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.CardPools;
using SlimeBoss.SlimeBossCode.Core;

namespace SlimeBoss.SlimeBossCode.Cards.Token;

[Pool(typeof(TokenCardPool))]
public class SlimeCrush : SlimeBossCardModel
{
    public SlimeCrush() : base(4, CardType.Attack, CardRarity.Token, TargetType.AnyEnemy)
    {
        WithDamage(35);
        WithKeywords(CardKeyword.Ethereal, CardKeyword.Exhaust);
    }

    public override TargetType TargetType => IsUpgraded ? TargetType.AllEnemies : TargetType.AnyEnemy;

    protected override Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        return CommonActions.CardAttack(this, cardPlay).Execute(ctx);
    }
}