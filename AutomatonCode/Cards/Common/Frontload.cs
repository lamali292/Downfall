using Automaton.AutomatonCode.Cards.Token;
using Automaton.AutomatonCode.Core;
using Automaton.AutomatonCode.Interfaces;
using Automaton.AutomatonCode.Powers;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Automaton.AutomatonCode.Cards.Common;

[Pool(typeof(AutomatonCardPool))]
public class Frontload : AutomatonCardModel, IEncodable
{
    public Frontload() : base(2, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        WithBlock(8, 3);
        WithTip(CardKeyword.Retain);
        WithPower<FrontloadPower>(1, false);
    }
    
    protected override Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
        => CommonActions.ApplySelf<FrontloadPower>(ctx, this);

    public Task PlayEncodableEffect(PlayerChoiceContext ctx, CardPlay cardPlay, EncodeContext encodeContext)
      => CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);
 
}