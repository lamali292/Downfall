using Automaton.AutomatonCode.Interfaces;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace Automaton.AutomatonCode.Cards.Token;

[Pool(typeof(TokenCardPool))]
public class Terminator : AutomatonCardModel,
    IEncodable
{
    public Terminator() : base(1, CardType.Skill, CardRarity.Token, TargetType.Self)
    {
        WithCostUpgradeBy(-1);
        WithTip(StaticHoverTip.ReplayStatic);
    }

    public Task PlayEncodableEffect(PlayerChoiceContext ctx, CardPlay cardPlay, EncodeContext encodeContext)
    {
        return Task.CompletedTask;
    }

    // Todo in compile
    public Task OnCompile(PlayerChoiceContext ctx, FunctionCard card, CardPlay cardPlay,
        bool forGameplay)
    {
        if (!forGameplay) return Task.CompletedTask;
        /*
        if (compileContext.IsLast)
            card.BaseReplayCount += 1;
            */
        return Task.CompletedTask;
    }
}