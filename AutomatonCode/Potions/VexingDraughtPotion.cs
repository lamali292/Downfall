using Automaton.AutomatonCode.Core;
using Automaton.AutomatonCode.CustomEnums;
using BaseLib.Utils;
using Downfall.DownfallCode.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Automaton.AutomatonCode.Potions;

[Pool(typeof(AutomatonPotionPool))]
public class VexingDraughtPotion : AutomatonPotionModel
{
    public VexingDraughtPotion() : base(PotionRarity.Common, PotionUsage.CombatOnly, TargetType.AnyPlayer)
    {
        WithPower<StrengthPower>(2);
        WithPower<DexterityPower>(2);
        WithCards(2);
        WithTip<Burn>();
        WithTip(AutomatonTip.Stash);
    }

    protected override async Task OnUse(PlayerChoiceContext ctx, Creature? target)
    {
        var player = target?.Player;
        if (player == null) return;
        await MyCommonActions.Apply<StrengthPower>(ctx, this, target);
        await MyCommonActions.Apply<DexterityPower>(ctx, this, target);
        await StashCmd.Stash<Burn>(ctx, player, DynamicVars.Cards.IntValue);
    }
}