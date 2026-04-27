using Automaton.AutomatonCode.Core;
using Automaton.AutomatonCode.CustomEnums;
using Automaton.AutomatonCode.Powers;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Automaton.AutomatonCode.Cards.Uncommon;

[Pool(typeof(AutomatonCardPool))]
public class Optimize : AutomatonCardModel
{
    public Optimize() : base(0, CardType.Power, CardRarity.Uncommon, TargetType.None)
    {
        WithTip(AutomatonTip.Encode);
        WithPower<OptimizePower>(3, 2);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<OptimizePower>(ctx, this);
    }
}