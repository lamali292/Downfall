using BaseLib.Utils;
using Champ.ChampCode.Core;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Champ.ChampCode.Relics;

[Pool(typeof(ChampRelicPool))]
public class AmuletOfUnyielding : ChampRelicModel
{
    public override RelicRarity Rarity => RelicRarity.Ancient;

    public override async Task AfterPowerAmountChanged(PlayerChoiceContext ctx, PowerModel power, decimal amount, Creature? applier,
        CardModel? cardSource)
    {
        if (power.Owner != Owner.Creature || power is not VigorPower vigor || amount > 0) return;
        var a = -amount / 12;
        if (a <=  0) return;
        await PowerCmd.Apply<StrengthPower>(ctx, Owner.Creature, a, Owner.Creature, null);
    }
}