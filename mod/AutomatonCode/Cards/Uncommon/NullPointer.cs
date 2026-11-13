using Automaton.AutomatonCode.Core;
using Automaton.AutomatonCode.Compile;
using Automaton.AutomatonCode.Encode;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Automaton.AutomatonCode.Cards.Uncommon;

[Pool(typeof(AutomatonCardPool))]
public class NullPointer : AutomatonCardModel
{
    public NullPointer() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        WithEncode<BlockEncode>();
        WithEncode<DamageEncode>();
        WithCompile<FunctionCostCompile>();
        WithDamage(10, 3);
        WithBlock(10, 3);
        WithEnergy(3);
    }

    protected override Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        return EncodeOutcome.EncodePlayEffect(this, ctx, cardPlay);
    }
}