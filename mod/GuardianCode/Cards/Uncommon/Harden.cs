using BaseLib.Utils;
using Guardian.GuardianCode.Core;
using Guardian.GuardianCode.CustomEnums;
using Guardian.GuardianCode.Interfaces;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Guardian.GuardianCode.Cards.Uncommon;

[Pool(typeof(GuardianCardPool))]
public class Harden : GuardianCardModel, ITickCard
{
    public Harden() : base(2, CardType.Power, CardRarity.Uncommon, TargetType.Self)
    {
        WithPower<PlatingPower>(4);
        WithPower<ThornsPower>(2, false);
        WithKeyword(GuardianKeyword.Volatile, UpgradeType.Remove);
        WithTip(GuardianTip.Tick);
    }

    public async Task OnTick(PlayerChoiceContext ctx)
    {
        await CommonActions.ApplySelf<ThornsPower>(ctx, this);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<PlatingPower>(ctx, this);
    }
}