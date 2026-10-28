using Collector.CollectorCode.CustomEnums;
using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using Collector.CollectorCode.Core;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

using Collector.CollectorCode.Cards.Token;
using Collector.CollectorCode.Powers;

namespace Collector.CollectorCode.Cards.Rare;

[Pool(typeof(CollectorCardPool))]
public class AstralFlames : CollectorCardModel
{
    public AstralFlames() : base(2, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
        WithKindle(5, 2);
        WithPower<AstralFlamesPower>(3, 1, false);
        WithTip(CollectorKeyword.Torchhead);
    }

    protected override Artist Artist => Artist.Get<Opal>();

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await TorchheadCmd.Kindle(ctx, this);
        await CommonActions.ApplySelf<AstralFlamesPower>(ctx, this);
    }
}
