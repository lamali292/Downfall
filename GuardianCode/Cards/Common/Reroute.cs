using BaseLib.Utils;
using Guardian.GuardianCode.Core;
using Guardian.GuardianCode.CustomEnums;
using Guardian.GuardianCode.Interfaces;
using Guardian.GuardianCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Guardian.GuardianCode.Cards.Common;

[Pool(typeof(GuardianCardPool))]
public class Reroute : GuardianCardModel, IGemSocketCard
{
    public Reroute() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
        WithDamage(9, 3);
        this.WithPower<ReroutePower>(1, false);
        WithTip(GuardianTip.Stasis);
    }

    public int GemSlots => 1;

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
        await CommonActions.ApplySelf<ReroutePower>(ctx, this);
    }
}