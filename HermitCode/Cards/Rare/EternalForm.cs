using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using Hermit.HermitCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Hermit.HermitCode.Cards.Rare;

public sealed class EternalForm : HermitCardModel
{
    public EternalForm() : base(3, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
        this.WithPower<EternalPower>(1, false);
        WithKeyword(CardKeyword.Ethereal, UpgradeType.Remove);
        WithEnergyTip();
    }

    protected override Artist Artist => Artist.Get<AlexMdle>();

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await CommonActions.ApplySelf<EternalPower>(ctx, this);
    }
}