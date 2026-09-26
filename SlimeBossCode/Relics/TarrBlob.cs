using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using SlimeBoss.SlimeBossCode.Core;

namespace SlimeBoss.SlimeBossCode.Relics;

[Obsolete]
[Pool(typeof(SlimeBossRelicPool))]
public class TarrBlob() : SlimeBossRelicModel(RelicRarity.Ancient, false)
{
    public override Task BeforeHandDraw(Player player, PlayerChoiceContext ctx, ICombatState combatState)
    {
        throw new NotImplementedException();
    }
}