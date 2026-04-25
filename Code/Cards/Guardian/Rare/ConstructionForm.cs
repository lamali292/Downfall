using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Abstract.CardModels;
using Downfall.Code.Powers.Guardian;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Downfall.Code.Cards.Guardian.Rare;

[Pool(typeof(GuardianCardPool))]
public class ConstructionForm : GuardianCardModel
{
    public ConstructionForm() : base(3, CardType.Power, CardRarity.Rare, TargetType.None)
    {
        WithPower<BufferPower>(2);
        WithKeyword(CardKeyword.Ethereal, UpgradeType.Remove);
        WithTip(typeof(StrengthPower));
        WithPower<ConstructionFormPower>(1);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<BufferPower>(ctx, this);
        await CommonActions.ApplySelf<ConstructionFormPower>(ctx, this);
    }
}