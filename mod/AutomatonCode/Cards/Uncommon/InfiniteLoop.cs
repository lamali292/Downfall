using Automaton.AutomatonCode.Compile;
using Automaton.AutomatonCode.Core;
using Automaton.AutomatonCode.Encode;
using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Automaton.AutomatonCode.Cards.Uncommon;

[Pool(typeof(AutomatonCardPool))]
public class InfiniteLoop : AutomatonCardModel
{
    public InfiniteLoop() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        WithEncode<DamageEncode>();
        WithCompile<InfiniteLoopCompile>();
        WithDamage(6);
        WithVar("Increase", 2, 2);
    }

    protected override Artist Artist => Artist.Get<Opal>();

    protected override Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        return EncodeOutcome.EncodePlayEffect(this, ctx, cardPlay);
    }
}