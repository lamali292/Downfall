using Hermit.HermitCode.CustomEnums;
using Hermit.HermitCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Hermit.HermitCode.Cards.Uncommon;

public sealed class SmokingBarrel : HermitCardModel
{
    public SmokingBarrel() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
    {
        this.WithPower<SmokingBarrelPower>(3, 1, false);
        WithTip(HermitKeywords.DeadOn);
        this.WithTip<VigorPower>();
    }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await PowerCmd.Apply<SmokingBarrelPower>(ctx, Owner.Creature, DynamicVars["BigShotPower"].BaseValue,
            Owner.Creature,
            this);
    }
}