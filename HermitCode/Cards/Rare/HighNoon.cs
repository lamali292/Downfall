using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using Hermit.HermitCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;

namespace Hermit.HermitCode.Cards.Rare;

public sealed class HighNoon : HermitCardModel
{
    public HighNoon() : base(2, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
        this.WithPower<HighNoonPower>(1, false);
        this.WithPower<HighNoonDefendPower>(0,1, false);
        WithTip(StaticHoverTip.ReplayStatic);
    }

    protected override Artist Artist => Artist.Get<AlexMdle>();

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await CommonActions.ApplySelf<HighNoonPower>(ctx, this);
        await CommonActions.ApplySelf<HighNoonDefendPower>(ctx, this);
    }
}