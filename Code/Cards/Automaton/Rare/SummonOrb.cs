using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Cards.CardModels;
using Downfall.Code.Extensions;
using Downfall.Code.Powers.Automaton;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace Downfall.Code.Cards.Automaton.Rare;

[Pool(typeof(AutomatonCardPool))]
public class SummonOrb : AutomatonCardModel
{
    public SummonOrb() : base(2, CardType.Power, CardRarity.Rare, TargetType.None)
    {
        WithPower<SummonOrbPower>(3);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<SummonOrbPower>(this, DynamicVars.Power<SummonOrbPower>().BaseValue);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Power<SummonOrbPower>().UpgradeValueBy(1);
    }
}