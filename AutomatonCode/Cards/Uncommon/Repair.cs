using Automaton.AutomatonCode.Core;
using Automaton.AutomatonCode.Powers;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Automaton.AutomatonCode.Cards.Uncommon;

[Pool(typeof(AutomatonCardPool))]
public class Repair : AutomatonCardModel
{
    public Repair() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        this.WithPower<SelfRepairPower>(7, 3, false);
    }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<SelfRepairPower>(ctx, this);
    }
}