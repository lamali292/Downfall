using BaseLib.Utils;
using Collector.CollectorCode.Cards.Token;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Encounters;
using MegaCrit.Sts2.Core.Models.Powers;
namespace Collector.CollectorCode.Cards.Collectibles;

public class SoulNexusCard : Collectible<SoulNexusElite>
{
    public SoulNexusCard() : base(2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy, 0.45f)
    {
        WithPower<DebilitatePower>(2, 1, false);
        WithDamage(18, 2);
        WithTip<VulnerablePower>();
        WithTip<WeakPower>();
    }
    
    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
        await CommonActions.Apply<DebilitatePower>(ctx, this, cardPlay);
    }
}
