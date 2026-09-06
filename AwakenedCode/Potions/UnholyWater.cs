using Awakened.AwakenedCode.Core;
using Awakened.AwakenedCode.CustomEnums;
using Awakened.AwakenedCode.Powers;
using BaseLib.Utils;
using Downfall.DownfallCode.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Awakened.AwakenedCode.Potions;

[Pool(typeof(AwakenedPotionPool))]
public class UnholyWater : AwakenedPotionModel
{
    public UnholyWater() : base(PotionRarity.Common, PotionUsage.CombatOnly, TargetType.AnyEnemy)
    {
        WithPower<ManaburnPower>(7);
        WithTip(AwakenedTip.Drained);
    }
    
    protected override async Task OnUse(PlayerChoiceContext ctx, Creature? target)
    {
        await MyCommonActions.Apply<ManaburnPower>(ctx, this, target);
    }
}