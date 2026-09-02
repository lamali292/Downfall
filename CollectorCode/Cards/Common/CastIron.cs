using BaseLib.Extensions;
using BaseLib.Utils;
using Collector.CollectorCode.Cards.Token;
using Collector.CollectorCode.Core;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Cards;
namespace Collector.CollectorCode.Cards.Common;

[Pool(typeof(CollectorCardPool))]
public class CastIron : CollectorCardModel
{
    public CastIron() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        //WithTip<Burn>();
        WithVar("Quantity", 2);
        
        WithTips(e =>
            e.IsUpgraded
                ? HoverTipFactory.FromCardWithCardHoverTips<Ember>()
                : HoverTipFactory.FromCardWithCardHoverTips<Burn>());
        WithVars(new SummonVar(4));
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)//Todo: Finish this later
    {
        await CollectorCmd.SummonTorchhead(ctx, Owner, DynamicVars.Summon.IntValue, this);
    }

}