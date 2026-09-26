using BaseLib.Utils;
using Downfall.DownfallCode.Compatibility;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using SlimeBoss.SlimeBossCode.Core;
using SlimeBoss.SlimeBossCode.CustomEnums;
using SlimeBoss.SlimeBossCode.Slimes;

namespace SlimeBoss.SlimeBossCode.Relics;

[Pool(typeof(SlimeBossRelicPool))]
public class VeryMagnificentBowlerHat : SlimeBossRelicModel, IModifyDamageMultiplicative
{
    public VeryMagnificentBowlerHat() : base(RelicRarity.Starter)
    {
        WithTip(SlimeBossTip.Command);
        WithSlimeTip<BruiserSlime>();
    }

    public override async Task BeforeHandDraw(Player player, PlayerChoiceContext ctx, ICombatState combatState)
    {
        if (Owner.PlayerCombatState is not { TurnNumber: 1 } || player != Owner) return;
        await SlimeBossCmd.Split<BruiserSlime>(ctx, player);
    }

    public decimal ModifyDamageMultiplicativeCompability(Creature? target, decimal amount, ValueProp props,
        Creature? dealer, CardModel? cardSource, CardPlay? cardPlay)
    {
        return dealer?.Monster is BruiserSlime slime && slime.PetOwner == Owner.Creature ? 2m : 1m;
    }
}
