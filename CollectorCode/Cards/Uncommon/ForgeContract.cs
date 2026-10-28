using BaseLib.Utils;
using Collector.CollectorCode.Core;
using Collector.CollectorCode.CustomEnums;
using Collector.CollectorCode.Extensions;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Collector.CollectorCode.Cards.Uncommon;

[Pool(typeof(CollectorCardPool))]
public class ForgeContract : CollectorCardModel
{
    public ForgeContract() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
        WithDamage(6, 2);
        WithTorchheadDamage(6, 2);
        WithTip(CollectorKeyword.Torchhead);
    }
    

    protected override Artist Artist => Artist.Get<Opal>();
    
    protected override bool NeedsTargetBesidesTorchhead => true;

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
        await Cmd.CustomScaledWait(0.1f, 0.3f);
            await TorchheadCmd.Attack(ctx, this, cardPlay);
    }
}