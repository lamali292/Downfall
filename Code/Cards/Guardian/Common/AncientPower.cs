using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Abstract.CardModels;
using Downfall.Code.Powers.Downfall;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Downfall.Code.Cards.Guardian.Common;

[Pool(typeof(GuardianCardPool))]
public class AncientPower : GuardianCardModel
{
    public AncientPower() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        WithPower<TemporaryStrengthUpPower>(3, 1);
        WithPower<TemporaryDexterityUpPower>(3, 1);
    }

    public override int GemSlots => 1;

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<TemporaryStrengthUpPower>(ctx, this);
        await CommonActions.ApplySelf<TemporaryDexterityUpPower>(ctx, this);
    }
}