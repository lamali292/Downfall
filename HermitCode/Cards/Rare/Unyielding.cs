using BaseLib.Utils;
using Hermit.HermitCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Hermit.HermitCode.Cards.Rare;

public class Unyielding : HermitCardModel
{
    public Unyielding() : base(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        WithBlock(5, 3);
        this.WithTip<VulnerablePower>();
        this.WithPower<UnyieldingPower>(1, false);
    }
    

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await CommonActions.CardBlock(this, cardPlay);
        await CommonActions.ApplySelf<UnyieldingPower>(ctx, this);
    }
}