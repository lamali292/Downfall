using BaseLib.Utils;
using Guardian.GuardianCode.Core;
using Guardian.GuardianCode.CustomEnums;
using Guardian.GuardianCode.Extensions;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Guardian.GuardianCode.Cards.Uncommon;

[Pool(typeof(GuardianCardPool))]
public class TimeBomb : GuardianCardModel
{
    public TimeBomb() : base(2, CardType.Attack, CardRarity.Uncommon, TargetType.AllEnemies)
    {
        WithDamage(14, 4);
        WithKeyword(CardKeyword.Exhaust);
        this.WithAccelerate(1);
        WithTip(GuardianTip.Stasis);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
        await GuardianCmd.Accelerate(ctx, this, AccelerateType.All);
    }
}