using BaseLib.Utils;
using Collector.CollectorCode.Cards.Token;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Encounters;
using MegaCrit.Sts2.Core.Models.Powers;
namespace Collector.CollectorCode.Cards.Collectibles;

public class ByrdonisCard : Collectible<ByrdonisElite>
{
    public ByrdonisCard() : base(3, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy, 0.3f)
    {
        WithDamage(7, 2);
        WithRepeat(3);
        WithPower<StrengthPower>(2, 1);
    }
    
    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay, DynamicVars.Repeat.IntValue).Execute(ctx);
        await CommonActions.ApplySelf<StrengthPower>(ctx, this);
    }
}
