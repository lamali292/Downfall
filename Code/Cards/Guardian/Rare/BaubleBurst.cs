using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Abstract.CardModels;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Downfall.Code.Cards.Guardian.Rare;

[Pool(typeof(GuardianCardPool))]
public class BaubleBurst : GuardianCardModel
{
    public BaubleBurst() : base(2, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
    {
    }

    public override int GemSlots => IsUpgraded ? 2 : 1;

    // TODO: Implement
    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
    }


    protected override void OnUpgrade()
    {
    }
}