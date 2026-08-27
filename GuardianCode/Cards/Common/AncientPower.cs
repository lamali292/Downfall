using BaseLib.Abstracts;
using BaseLib.Utils;
using Guardian.GuardianCode.Core;
using Guardian.GuardianCode.Interfaces;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Guardian.GuardianCode.Cards.Common;

[Pool(typeof(GuardianCardPool))]
public class AncientPower : GuardianCardModel, IGemSocketCard
{
    public AncientPower() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        WithPower<AncientStrengthPower>(3, 1, false);
        WithPower<AncientDexterityPower>(3, 1, false);
        WithTip<StrengthPower>();
        WithTip<DexterityPower>();
    }

    public int GemSlots => 1;

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<AncientStrengthPower>(ctx, this);
        await CommonActions.ApplySelf<AncientDexterityPower>(ctx, this);
    }
}

public class AncientStrengthPower : CustomTemporaryPowerModelWrapper<AncientPower, StrengthPower>;

public class AncientDexterityPower : CustomTemporaryPowerModelWrapper<AncientPower, DexterityPower>;