using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Cards.CardModels;
using Downfall.Code.Powers.Awakened;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Void = MegaCrit.Sts2.Core.Models.Cards.Void;

namespace Downfall.Code.Cards.Awakened.Rare;

[Pool(typeof(AwakenedCardPool))]
public class AncestralGrounds : AwakenedCardModel
{
    public AncestralGrounds() : base(2, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        WithBlock(12);
        WithVar("Energy", 2, 1);
        WithEnergyTip();
        WithTip(typeof(Void));
    }


    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardBlock(this, cardPlay);
        if (IsUpgraded)
            await CommonActions.ApplySelf<AncestralGroundsUpgradedPower>(this, 2);
        else
            await CommonActions.ApplySelf<AncestralGroundsPower>(this, 2);
    }
}