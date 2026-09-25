using BaseLib.Utils;
using Downfall.DownfallCode.Commands;
using Gremlins.GremlinsCode.Core;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Gremlins.GremlinsCode.Relics;

[Pool(typeof(GremlinsRelicPool))]
public class ImpeccablePecs : GremlinsRelicModel
{
    public ImpeccablePecs() : base(RelicRarity.Uncommon)
    {
        WithPower<StrengthPower>(1);
    }

    public override async Task BeforeHandDraw(Player player, PlayerChoiceContext ctx, ICombatState combatState)
    {
        if (player != Owner || Owner.PlayerCombatState is not { TurnNumber: 1 }) return;
        await MyCommonActions.ApplySelf<StrengthPower>(ctx, this);
    }


    public override async Task AfterPowerAmountChanged(PlayerChoiceContext ctx, PowerModel power, decimal amount, Creature? applier,
        CardModel? cardSource)
    {
        if (power.Owner != Owner.Creature || power is not StrengthPower) return;
        await GremlinsCmd.GainTempHp(ctx, Owner.Creature, amount);
    }
}