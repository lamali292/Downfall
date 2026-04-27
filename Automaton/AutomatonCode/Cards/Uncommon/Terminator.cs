using Automaton.AutomatonCode.Cards.Token;
using Automaton.AutomatonCode.Core;
using Automaton.AutomatonCode.CustomEnums;
using Automaton.AutomatonCode.Interfaces;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Automaton.AutomatonCode.Cards.Uncommon;

[Pool(typeof(AutomatonCardPool))]
public class Terminator : AutomatonCardModel,
    IEncodable, ICompilable
{
    public Terminator() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithTip(AutomatonTip.Encode);
        WithTip(AutomatonTip.Compile);
        WithCostUpgradeBy(-1);
    }

    public Task OnCompile(PlayerChoiceContext ctx, FunctionCard card, CardPlay cardPlay, CompileContext compileContext,
        bool forGameplay)
    {
        if (!forGameplay) return Task.CompletedTask;
        if (compileContext.IsLast)
            card.BaseReplayCount += 1;
        return Task.CompletedTask;
    }

    public Task PlayEncodableEffect(PlayerChoiceContext ctx, CardPlay cardPlay, EncodeContext encodeContext)
    {
        return Task.CompletedTask;
    }
}