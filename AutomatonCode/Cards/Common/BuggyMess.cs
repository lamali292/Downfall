using Automaton.AutomatonCode.Core;
using Automaton.AutomatonCode.Encode;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Cards;

namespace Automaton.AutomatonCode.Cards.Common;

[Pool(typeof(AutomatonCardPool))]
public class BuggyMess : AutomatonCardModel
{
    public BuggyMess() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        WithEncode<EnergyEncode>();
        WithEncode<DazedEncode>();
        WithEnergyTip();
        WithTip<Dazed>();
        WithCostUpgradeBy(-1);
        WithEnergy(1);
        WithVar("Dazed", 1);
    }

    protected override Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        return EncodeOutcome.EncodePlayEffect(this, ctx, cardPlay);
    }
}