using Automaton.AutomatonCode.Core;
using Automaton.AutomatonCode.Encode;
using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.ValueProps;

namespace Automaton.AutomatonCode.Cards.Token;

[Pool(typeof(TokenCardPool))]
public class Constructor : AutomatonCardModel
{
    public Constructor() : base(1, CardType.Skill, CardRarity.Token, TargetType.Self)
    {
        WithEncode<BlockEncode>();
        WithEncode<StartBlockEncode>();
        WithBlock(5, 2);
        WithVars(new BlockVar("ExtraBlock", 5, BlockProps.card).WithUpgrade(2));
    }

    protected override Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        return EncodeOutcome.EncodePlayEffect(this, ctx, cardPlay);
    }
}