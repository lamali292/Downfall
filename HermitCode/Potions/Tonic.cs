using Downfall.DownfallCode.Commands;
using Hermit.HermitCode.Core;
using Hermit.HermitCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Hermit.HermitCode.Potions;

public class Tonic : HermitPotionModel
{
    public Tonic() : base(PotionRarity.Uncommon, PotionUsage.CombatOnly, TargetType.Self)
    {
        WithPower<RuggedPower>(1);
    }
    
    protected override Task OnUse(PlayerChoiceContext ctx, Creature? target)
    {
        return MyCommonActions.ApplySelf<RuggedPower>(ctx, this);
    }
}