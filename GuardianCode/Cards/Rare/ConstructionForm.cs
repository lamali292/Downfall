using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using Guardian.GuardianCode.Core;
using Guardian.GuardianCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Guardian.GuardianCode.Cards.Rare;

[Pool(typeof(GuardianCardPool))]
public class ConstructionForm : GuardianCardModel
{
    public ConstructionForm() : base(3, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
        WithPower<BufferPower>(2);
        WithKeyword(CardKeyword.Ethereal, UpgradeType.Remove);
        this.WithTip<StrengthPower>();
        this.WithPower<ConstructionFormPower>(1, false);
    }

    protected override Artist Artist => Artist.Get<CartesianCanvas>();

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<BufferPower>(ctx, this);
        await CommonActions.ApplySelf<ConstructionFormPower>(ctx, this);
    }
}