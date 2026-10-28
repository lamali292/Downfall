using BaseLib.Extensions;
using BaseLib.Utils;
using Collector.CollectorCode.Cards.Token;
using Collector.CollectorCode.CustomEnums;
using Collector.CollectorCode.Patches;
using Collector.CollectorCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Encounters;
using Collector.CollectorCode.Core;
namespace Collector.CollectorCode.Cards.Collectibles;

public class WaterfallGiantCard : Collectible<WaterfallGiantBoss>
{
    public WaterfallGiantCard() : base(3, CardType.Skill, CardRarity.Rare, TargetType.Self, 0.88f)
    {
        
        WithPower<WaterfallGiantCardPower>(3, false);
        WithPower<BlisterPower>(6, 2);
        WithKindle(6, 3);
        WithKeyword(CollectorKeyword.Smolder);
    }

    protected override async Task OnSmolder(PlayerChoiceContext ctx)
    {
        await TorchheadCmd.Kindle(ctx, this);
        (await CommonActions.ApplySelf<WaterfallGiantCardPower>(ctx, this))?.SetBlister(DynamicVars.Power<BlisterPower>().BaseValue);
    }

}