using BaseLib.Utils;
using Downfall.DownfallCode.Powers;
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
        this.WithPower<TemporaryStrengthUpPower>(3, 1, false);
        this.WithPower<TemporaryDexterityUpPower>(3, 1, false);
        this.WithTip<StrengthPower>();
        this.WithTip<DexterityPower>();
    }

    public int GemSlots => 1;

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<TemporaryStrengthUpPower>(ctx, this);
        await CommonActions.ApplySelf<TemporaryDexterityUpPower>(ctx, this);
    }
}