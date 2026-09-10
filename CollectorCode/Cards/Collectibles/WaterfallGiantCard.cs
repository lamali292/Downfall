using BaseLib.Extensions;
using BaseLib.Utils;
using Collector.CollectorCode.Cards.Token;
using Collector.CollectorCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Encounters;
namespace Collector.CollectorCode.Cards.Collectibles;

public class WaterfallGiantCard : Collectible<WaterfallGiantBoss>
{
    public WaterfallGiantCard() : base(2, CardType.Skill, CardRarity.Rare, TargetType.AnyEnemy, 0.3f)
    {
        WithPower<WaterfallGiantCardPower>(3, false);
        WithPower<MiasmaPower>(40, 10);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        (await CommonActions.ApplySelf<WaterfallGiantCardPower>(ctx, this))?.SetMiasma(DynamicVars.Power<MiasmaPower>().BaseValue);
    }
}