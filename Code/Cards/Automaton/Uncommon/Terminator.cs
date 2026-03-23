using BaseLib.Utils;
using Downfall.Code.Cards.Automaton.Token;
using Downfall.Code.Cards.CardModels;
using Downfall.Code.Character.Automaton;
using Downfall.Code.Commands;
using Downfall.Code.Keywords;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;

namespace Downfall.Code.Cards.Automaton.Uncommon;

[Pool(typeof(AutomatonCardPool))]
public sealed class Terminator() : AutomatonCardModel(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self), IEncodable, ICompilable
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(DownfallKeywords.Encode),
        HoverTipFactory.FromKeyword(DownfallKeywords.Compile),
    ];

    public Task PlayEncodableEffect(PlayerChoiceContext ctx, CardPlay cardPlay, EncodeContext encodeContext)
        => Task.CompletedTask;

    public Task OnCompile(PlayerChoiceContext ctx, FunctionCard card, CardPlay cardPlay, CompileContext compileContext, bool forGameplay)
    {
        if (!forGameplay) return Task.CompletedTask;
        if (compileContext.IsLast)
            card.BaseReplayCount += 1;
        return Task.CompletedTask;
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}