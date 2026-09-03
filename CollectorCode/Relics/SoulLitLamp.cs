using BaseLib.Extensions;
using BaseLib.Utils;
using Collector.CollectorCode.Core;
using Collector.CollectorCode.CustomEnums;
using Collector.CollectorCode.Events;
using Downfall.DownfallCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Collector.CollectorCode.Relics;

[Pool(typeof(CollectorRelicPool))]
public class SoulLitLamp : CollectorRelicModel, IOnPyre
{
    public SoulLitLamp() : base(RelicRarity.Uncommon)
    {
        WithPower<SoulBurnPower>(3);
        WithEnergy(2);
        WithTip(CollectorKeyword.Pyre);
    }

    public async Task OnPyre(PlayerChoiceContext ctx, CardModel card, CardModel pyred)
    {
        if (pyred._energyCost != null && pyred._energyCost.GetAmountToSpend() >= DynamicVars.Energy.BaseValue)
        {
            await PowerCmd.Apply<SoulBurnPower>(ctx,
                card.CombatState!.HittableEnemies,
                DynamicVars.Power<SoulBurnPower>().BaseValue,
                Owner.Creature,
                null,
                false);
            Flash();
        }
    }
}