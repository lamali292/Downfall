using Champ.ChampCode.Core;
using Champ.ChampCode.Powers;
using BaseLib.Utils;
using Downfall.DownfallCode.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Champ.ChampCode.Potions;

[Pool(typeof(ChampPotionPool))]
public class BrewOfSharpness : ChampPotionModel
{
    public BrewOfSharpness() : base(PotionRarity.Common, PotionUsage.CombatOnly, TargetType.Self)
    {
        WithPower<CounterPower>(25);
    }

    protected override Task OnUse(PlayerChoiceContext ctx, Creature? target)
    {
        return MyCommonActions.ApplySelf<CounterPower>(ctx, this);
    }
}