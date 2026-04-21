using BaseLib.Extensions;
using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Abstract.CardModels;
using Downfall.Code.Powers.Collector;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Downfall.Code.Cards.Collector.Uncommon;

[Pool(typeof(CollectorCardPool))]
public class Empower : CollectorCardModel
{
    public Empower() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.None)
    {
        WithTip(typeof(StrengthPower));
        WithVars(new IntVar("Turns", 2).WithUpgrade(1));
    }

    protected override bool HasEnergyCostX => true;

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var amount = ResolveEnergyXValue();
        var a = await CommonActions.ApplySelf<EmpowerPower>(this, amount);
        if (IsUpgraded && a is { } empowerPower) empowerPower.SetTurns(DynamicVars["Turns"].BaseValue);
    }
}