using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Cards.Automaton.Token;
using Downfall.Code.Cards.CardModels;
using Downfall.Code.Interfaces;
using Downfall.Code.Keywords;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Downfall.Code.Cards.Automaton.Uncommon;

[Pool(typeof(AutomatonCardPool))]
public class Terminator : AutomatonCardModel,
    IEncodable, ICompilable
{
    public Terminator() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithTip(DownfallKeyword.Encode);
        WithTip(DownfallKeyword.Compile);
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


    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}