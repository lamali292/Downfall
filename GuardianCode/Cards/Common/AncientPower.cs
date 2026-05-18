using BaseLib.Utils;
using Downfall.DownfallCode.Powers;
using Guardian.GuardianCode.Core;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Guardian.GuardianCode.Cards.Common;

[Pool(typeof(GuardianCardPool))]
public class AncientPower : GuardianCardModel
{
    public AncientPower() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        WithPower<TemporaryStrengthUpPower>(3, 1, false);
        WithPower<TemporaryDexterityUpPower>(3, 1, false);
        WithTip(typeof(StrengthPower));
        WithTip(typeof(DexterityPower));
    }

    public override int GemSlots => 1;

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<TemporaryStrengthUpPower>(ctx, this);
        await CommonActions.ApplySelf<TemporaryDexterityUpPower>(ctx, this);
    }
}