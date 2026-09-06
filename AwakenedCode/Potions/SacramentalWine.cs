using Awakened.AwakenedCode.Core;
using Awakened.AwakenedCode.Powers;
using BaseLib.Utils;
using Downfall.DownfallCode.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;

namespace Awakened.AwakenedCode.Potions;

[Pool(typeof(AwakenedPotionPool))]
public class SacramentalWine : AwakenedPotionModel
{
    public SacramentalWine() : base(PotionRarity.Uncommon, PotionUsage.CombatOnly, TargetType.AnyPlayer)
    {
        WithTip(StaticHoverTip.Block);
        WithPower<SacramentalWinePower>(3, false);
    }

    protected override async Task OnUse(PlayerChoiceContext ctx, Creature? target)
    {
        await MyCommonActions.Apply<SacramentalWinePower>(ctx, this, target);
    }
}