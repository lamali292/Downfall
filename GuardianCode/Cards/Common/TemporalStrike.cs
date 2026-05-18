using BaseLib.Utils;
using Guardian.GuardianCode.Core;
using Guardian.GuardianCode.CustomEnums;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Guardian.GuardianCode.Cards.Common;

[Pool(typeof(GuardianCardPool))]
public class TemporalStrike : GuardianCardModel
{
    public TemporalStrike() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
        WithDamage(7, 3);
        WithEnergy(1);
        WithTip(GuardianTip.Stasis);
    }

    public override int GemSlots => 1;

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
        if (GuardianCmd.GetStasisCount(Owner) == 0) return;
        await PlayerCmd.GainEnergy(DynamicVars.Energy.BaseValue, Owner);
    }
}