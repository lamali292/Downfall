using BaseLib.Utils;
using Collector.CollectorCode.Cards.Token;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Encounters;

namespace Collector.CollectorCode.Cards.Collectibles;


public class PhantasmalGardenerCard
    : Collectible<PhantasmalGardenersElite>
{
    public PhantasmalGardenerCard() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy, 0.3f)
    {
        WithBlock(7, 2);
        WithDamage(1, 1);
        WithRepeat(3);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardBlock(this, cardPlay);
        await CommonActions.CardAttack(this, cardPlay, DynamicVars.Repeat.IntValue).Execute(ctx);
    }
}