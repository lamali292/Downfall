using BaseLib.Extensions;
using BaseLib.Utils;
using Collector.CollectorCode.Core;
using Collector.CollectorCode.Powers;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Collector.CollectorCode.Cards.Uncommon;

[Pool(typeof(CollectorCardPool))]
public class Empower : CollectorCardModel
{
    public Empower() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
    {
        this.WithTip<StrengthPower>();
        WithVars(new IntVar("Turns", 2).WithUpgrade(1));
    }

    protected override Artist Artist => Artist.Get<Opal>();

    protected override bool HasEnergyCostX => true;

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var amount = ResolveEnergyXValue();
        var a = await CommonActions.ApplySelf<EmpowerPower>(ctx, this, amount);
        if (IsUpgraded && a is { } empowerPower) empowerPower.SetTurns(DynamicVars["Turns"].BaseValue);
    }
}