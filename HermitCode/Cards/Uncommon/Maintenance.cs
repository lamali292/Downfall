using BaseLib.Utils;
using Hermit.HermitCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Hermit.HermitCode.Cards.Uncommon;

public sealed class Maintenance : HermitCardModel
{
    public Maintenance() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        this.WithPower<MaintenanceStrikePower>(3, 1, false);
        WithPower<DexterityPower>(1, 1);
    }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await CommonActions.ApplySelf<MaintenanceStrikePower>(ctx, this);
        await CommonActions.ApplySelf<DexterityPower>(ctx, this);
        EnergyCost.UpgradeBy(1);
    }
}