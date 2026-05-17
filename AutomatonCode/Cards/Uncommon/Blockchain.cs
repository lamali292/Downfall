using Automaton.AutomatonCode.Cards.Token;
using Automaton.AutomatonCode.Core;
using Automaton.AutomatonCode.CustomEnums;
using Automaton.AutomatonCode.Interfaces;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Automaton.AutomatonCode.Cards.Uncommon;

[Pool(typeof(AutomatonCardPool))]
public class Blockchain : AutomatonCardModel, IEncodable,
    ICompilable
{
    public Blockchain() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithPower<BlurPower>(1);
        WithTip(AutomatonTip.Encode);
        WithTips(card => card.IsUpgraded ? [HoverTipFactory.Static(AutomatonTip.Compile)] : []);
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