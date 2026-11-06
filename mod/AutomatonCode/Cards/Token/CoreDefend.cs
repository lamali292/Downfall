using Automaton.AutomatonCode.Core;
using Automaton.AutomatonCode.CustomEnums;
using Automaton.AutomatonCode.Encode;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace Automaton.AutomatonCode.Cards.Token;

[Pool(typeof(TokenCardPool))]
public class CoreDefend : AutomatonCardModel
{
    public CoreDefend() : base(1, CardType.Skill, CardRarity.Basic, TargetType.Self)
    {
        WithEncode<BlockEncode>();
        WithBlock(5, 3);
        WithTags(CardTag.Defend);
        WithTags(AutomatonTag.Core);
    }
    
    public override bool CanBeGeneratedByModifiers => false;
    public override bool CanBeGeneratedInCombat => false;

    public override CardPoolModel VisualCardPool => _owner?.Character.CardPool ?? Pool;

    protected override Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        return EncodeOutcome.EncodePlayEffect(this, ctx, cardPlay);
    }
}