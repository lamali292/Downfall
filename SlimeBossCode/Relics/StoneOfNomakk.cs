using BaseLib.Utils;
using Downfall.DownfallCode.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using SlimeBoss.SlimeBossCode.Core;
using SlimeBoss.SlimeBossCode.Powers;

namespace SlimeBoss.SlimeBossCode.Relics;

[Pool(typeof(SlimeBossRelicPool))]
public class StoneOfNomakk : SlimeBossRelicModel
{
    public StoneOfNomakk() : base(RelicRarity.Common)
    {
        WithPower<PotencyPower>(1);
    }

    public override Task BeforeHandDraw(Player player, PlayerChoiceContext ctx, ICombatState combatState)
    {
        return combatState.RoundNumber > 1 || player != Owner
            ? Task.CompletedTask
            : MyCommonActions.ApplySelf<PotencyPower>(ctx, this);
    }
}