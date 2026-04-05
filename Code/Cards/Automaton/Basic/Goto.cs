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

namespace Downfall.Code.Cards.Automaton.Basic;

[Pool(typeof(AutomatonCardPool))]
public class Goto : AutomatonCardModel, ICompilable, IEncodable
{
    public Goto() : base(1, CardType.Skill, CardRarity.Basic, TargetType.Self)
    {
        WithCards(1, 1);
        WithVar("Compile", 1, 1);
        WithTip(DownfallKeyword.Encode);
        WithTip(DownfallKeyword.Compile);
    }

    public async Task OnCompile(PlayerChoiceContext ctx, FunctionCard function, CardPlay cardPlay,
        CompileContext compileContext,
        bool forGameplay)
    {
        if (!forGameplay) return;
        await PowerCmd.Apply<DrawCardsNextTurnPower>(
            Owner.Creature,
            DynamicVars["Compile"].BaseValue,
            Owner.Creature,
            this);
    }

    public async Task PlayEncodableEffect(PlayerChoiceContext ctx, CardPlay cardPlay, EncodeContext encodeContext)
    {
        await CardPileCmd.Draw(ctx, DynamicVars.Cards.BaseValue, cardPlay.Card.Owner);
    }
}