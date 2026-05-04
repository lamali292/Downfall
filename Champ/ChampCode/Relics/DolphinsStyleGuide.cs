using BaseLib.Utils;
using Champ.ChampCode.Core;
using Champ.ChampCode.Extensions;
using Champ.ChampCode.Stance;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Champ.ChampCode.Relics;

[Pool(typeof(ChampRelicPool))]
public class DolphinsStyleGuide : ChampRelicModel
{
    public override RelicRarity Rarity => RelicRarity.Uncommon;
    
    public override async Task BeforeTurnEnd(PlayerChoiceContext ctx, CombatSide side)
    {
        var creature = Owner.Creature;
        if (side != creature.Side || !Owner.IsInChampStance<ChampNoStance>()) return;
        await PowerCmd.Apply<DrawCardsNextTurnPower>(ctx, creature, 1, creature, null);
    }
}