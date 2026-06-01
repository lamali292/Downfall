using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using Hermit.HermitCode.CustomEnums;
using Hermit.HermitCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Hermit.HermitCode.Cards.Uncommon;

public sealed class TakeAim : HermitCardModel
{
    public TakeAim() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
    {
        WithKeyword(HermitKeywords.Concentrate, UpgradeType.Add);
        WithTip(HermitKeywords.Concentrate);
        this.WithPower<TakeAimPower>(1, false);
    }

    protected override Artist Artist => Artist.Get<AlexMdle>();

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await CommonActions.ApplySelf<TakeAimPower>(ctx, this);
    }
}