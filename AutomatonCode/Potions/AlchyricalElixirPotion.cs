using Automaton.AutomatonCode.Core;
using Automaton.AutomatonCode.Powers;
using BaseLib.Utils;
using Downfall.DownfallCode.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Automaton.AutomatonCode.Potions;

[Pool(typeof(AutomatonPotionPool))]
public class AlchyricalElixirPotion : AutomatonPotionModel
{
    public AlchyricalElixirPotion() : base(PotionRarity.Uncommon, PotionUsage.CombatOnly, TargetType.Self)
    {
        WithPower<AlchyricalElixirPower>(1);
    }

    protected override Task OnUse(PlayerChoiceContext ctx, Creature? target)
        => MyCommonActions.ApplySelf<AlchyricalElixirPower>(ctx, this);
}