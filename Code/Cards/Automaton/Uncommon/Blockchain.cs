using BaseLib.Extensions;
using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Abstract.CardModels;
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
public class Blockchain : AutomatonCardModel, IEncodable,
    ICompilable
{
    public Blockchain() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithPower<BlurPower>(1);
        WithTip(DownfallTip.Encode);
        WithTip(new TooltipSource(card =>
            card.IsUpgraded ? DownfallTip.Compile.ToHoverTip() : null!));
    }


    public async Task OnCompile(PlayerChoiceContext ctx, FunctionCard card, CardPlay cardPlay,
        CompileContext compileContext,
        bool forGameplay)
    {
        if (IsUpgraded)
            await CommonActions.ApplySelf<BlurPower>(ctx, this);
    }


    public async Task PlayEncodableEffect(PlayerChoiceContext ctx, CardPlay cardPlay, EncodeContext encodeContext)
    {
        await CommonActions.ApplySelf<BlurPower>(ctx, this);
    }
}