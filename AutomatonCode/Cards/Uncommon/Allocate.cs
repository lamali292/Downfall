using Automaton.AutomatonCode.Core;
using BaseLib.Utils;
using Downfall.DownfallCode.CustomEnums;
using Downfall.DownfallCode.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Automaton.AutomatonCode.Cards.Uncommon;

[Pool(typeof(AutomatonCardPool))]
public class Allocate : AutomatonCardModel
{
    public Allocate() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithTip(DownfallTip.Status);
        WithCostUpgradeBy(-1);
        WithEnergyTip();
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var statusCount = Owner.GetDraw().Count(c => c.Type == CardType.Status);
        if (statusCount > 0)
            await PlayerCmd.GainEnergy(statusCount, Owner);
    }
}