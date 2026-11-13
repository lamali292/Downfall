using Awakened.AwakenedCode.Core;
using Awakened.AwakenedCode.CustomEnums;
using BaseLib.Utils;
using Downfall.DownfallCode.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Awakened.AwakenedCode.Relics;

[Pool(typeof(AwakenedRelicPool))]
public class ShreddedDoll : AwakenedRelicModel
{
    public ShreddedDoll() : base(RelicRarity.Starter)
    {
        WithTip(AwakenedTip.Conjure);
        WithPower<RitualPower>(1);
    }

    public override async Task BeforeHandDraw(Player player, PlayerChoiceContext ctx, ICombatState combatState)
    {
        if (player != Owner) return;
        if (player.PlayerCombatState is { TurnNumber: 1 }) await MyCommonActions.ApplySelf<RitualPower>(ctx, this);
        Flash();
        await AwakenedCmd.Conjure(Owner);
    }
}