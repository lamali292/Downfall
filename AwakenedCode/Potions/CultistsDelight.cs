using Awakened.AwakenedCode.Core;
using Awakened.AwakenedCode.Powers;
using BaseLib.Utils;
using Downfall.DownfallCode.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Awakened.AwakenedCode.Potions;

[Pool(typeof(AwakenedPotionPool))]
public class CultistsDelight : AwakenedPotionModel
{
    public CultistsDelight() : base(PotionRarity.Rare, PotionUsage.CombatOnly, TargetType.AnyPlayer)
    {
        WithPower<CuriosityPower>(1);
    }

    protected override async Task OnUse(PlayerChoiceContext ctx, Creature? target)
    {
        await MyCommonActions.Apply<CuriosityPower>(ctx, this, target);
    }
}