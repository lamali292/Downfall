using Automaton.AutomatonCode.Core;
using Automaton.AutomatonCode.Extensions;
using BaseLib.Utils;
using Downfall.DownfallCode.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace Automaton.AutomatonCode.Cards.Uncommon;

[Pool(typeof(AutomatonCardPool))]
public class Allocate : AutomatonCardModel
{
    public Allocate() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithCostUpgradeBy(-1);
        WithEnergyTip();
        WithCalculatedVar("Status", 0, Calc);
    }

    private static decimal Calc(CardModel card, Creature? _)
    {
        return card.Owner.GetDraw().Count(c => c.Type == CardType.Status) +
               card.Owner.GetStash().Count(c => c.Type == CardType.Status);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var statusCount = ((CalculatedVar)DynamicVars["Status"]).Calculate(null);
        await PlayerCmd.GainEnergy(statusCount, Owner);
    }
}