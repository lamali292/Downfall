using Automaton.AutomatonCode.Core;
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
    public VexingDraughtPotion() : base(PotionRarity.Common, PotionUsage.CombatOnly, TargetType.Self)
    {
        WithPower<StrengthPower>(2);
        WithPower<DexterityPower>(2);
        WithTip(typeof(Burn));
    }

    protected override async Task OnUse(PlayerChoiceContext ctx, Creature? target)
    {
        await MyCommonActions.ApplySelf<StrengthPower>(ctx, this);
        await MyCommonActions.ApplySelf<DexterityPower>(ctx, this);
        await StashCmd.Stash<Burn>(Owner, 2);
    }
}