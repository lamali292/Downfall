using Automaton.AutomatonCode.Cards.Token;
using Automaton.AutomatonCode.Core;
using Automaton.AutomatonCode.CustomEnums;
using Automaton.AutomatonCode.Interfaces;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Automaton.AutomatonCode.Cards.Common;

[Pool(typeof(AutomatonCardPool))]
public class Frontload : AutomatonCardModel, ICompilable, IEncodable
{
    public Frontload() : base(2, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        WithBlock(8, 3);
        WithTip(CardKeyword.Retain);
    }

    public Task OnCompile(PlayerChoiceContext ctx, FunctionCard function, CardPlay cardPlay,
        CompileContext compileContext, bool forGameplay)
    {
        function.AddKeyword(CardKeyword.Retain);
        return Task.CompletedTask;
    }

    public async Task PlayEncodableEffect(PlayerChoiceContext ctx, CardPlay cardPlay, EncodeContext encodeContext)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);
    }


    public override void ApplyToFunctionPreview(FunctionCard card)
    {
        card.AddKeyword(CardKeyword.Retain);
    }
}