using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Cards.Automaton.Token;
using Downfall.Code.Cards.CardModels;
using Downfall.Code.Interfaces;
using Downfall.Code.Keywords;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Downfall.Code.Cards.Automaton.Uncommon;

[Pool(typeof(AutomatonCardPool))]
public class Boost : AutomatonCardModel, IEncodable,
    ICompilable
{
    public Boost() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithBlock(6);
        WithPower<StrengthPower>(1, 1);
        WithTip(DownfallKeyword.Encode);
        WithTip(DownfallKeyword.Compile);
    }


    public async Task OnCompile(PlayerChoiceContext ctx, FunctionCard card, CardPlay cardPlay,
        CompileContext compileContext,
        bool forGameplay)
    {
        await PowerCmd.Apply<StrengthPower>(Owner.Creature, DynamicVars.Strength.BaseValue, Owner.Creature, this);
    }

    public async Task PlayEncodableEffect(PlayerChoiceContext ctx, CardPlay cardPlay, EncodeContext encodeContext)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);
    }
}