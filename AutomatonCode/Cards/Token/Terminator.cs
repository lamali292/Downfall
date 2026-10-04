using Automaton.AutomatonCode.Core;
using Automaton.AutomatonCode.Encode;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace Automaton.AutomatonCode.Cards.Token;

[Pool(typeof(TokenCardPool))]
public class Terminator : AutomatonCardModel
{
    public Terminator() : base(1, CardType.Skill, CardRarity.Token, TargetType.Self)
    {
        WithEncode<ReplayAtEndEncode>();
        WithCostUpgradeBy(-1);
        WithTip(StaticHoverTip.ReplayStatic);
    }

    protected override Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        return EncodeOutcome.EncodePlayEffect(this, ctx, cardPlay);
    }
}