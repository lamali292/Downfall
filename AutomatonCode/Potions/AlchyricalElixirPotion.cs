using Automaton.AutomatonCode.Core;
using Automaton.AutomatonCode.Powers;
using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using Downfall.DownfallCode.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Automaton.AutomatonCode.Potions;

[Pool(typeof(AutomatonPotionPool))]
public class AlchyricalElixirPotion : AutomatonPotionModel
{
    public AlchyricalElixirPotion() : base(PotionRarity.Uncommon, PotionUsage.CombatOnly, TargetType.Self)
    {
        WithPower<AlchyricalElixirPower>(1, false);
    }

    protected override Artist Artist => Artist.Get<Chimedragon>();

    protected override Task OnUse(PlayerChoiceContext ctx, Creature? target)
    {
        return MyCommonActions.ApplySelf<AlchyricalElixirPower>(ctx, this);
    }
}