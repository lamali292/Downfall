using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Cards.CardModels;
using Downfall.Code.Commands;
using Downfall.Code.Extensions;
using Downfall.Code.Keywords;
using Downfall.Code.Powers.Awakened;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Downfall.Code.Cards.Awakened.Uncommon;

[Pool(typeof(AwakenedCardPool))]
public class StormRuler : AwakenedCardModel
{
    public StormRuler() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.None)
    {
        WithPower<StormRulerPower>(6);
        WithTip(DownfallKeyword.Conjure);
    }


    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(CombatState);
        await AwakenedCmd.Conjure(Owner, CombatState);
        await CommonActions.ApplySelf<StormRulerPower>(this, DynamicVars.Power<StormRulerPower>().BaseValue);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Power<StormRulerPower>().UpgradeValueBy(3);
    }
}