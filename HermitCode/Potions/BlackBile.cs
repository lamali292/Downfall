using BaseLib.Utils;
using Downfall.DownfallCode.Commands;
using Hermit.HermitCode.Core;
using Hermit.HermitCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Hermit.HermitCode.Potions;


public class BlackBile : HermitPotionModel
{

    public BlackBile() : base(PotionRarity.Common, PotionUsage.CombatOnly, TargetType.AnyEnemy)
    {
        WithPower<BruisePower>(6);
    }
    
    protected override Task OnUse(PlayerChoiceContext ctx, Creature? target)
    {
        return MyCommonActions.Apply<BruisePower>(ctx, this, target);
    }
}