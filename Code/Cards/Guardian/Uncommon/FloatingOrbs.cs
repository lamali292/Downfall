using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Abstract.CardModels;
using Downfall.Code.Powers.Guardian;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Downfall.Code.Cards.Guardian.Uncommon;

[Pool(typeof(GuardianCardPool))]
public class FloatingOrbs : GuardianCardModel
{
    public FloatingOrbs() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.None)
    {
        WithPower<FloatingOrbsPower>(3, 1);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<FloatingOrbsPower>(ctx, this);
    }
}