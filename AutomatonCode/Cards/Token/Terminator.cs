using Automaton.AutomatonCode.Core;
using Automaton.AutomatonCode.Interfaces;
using Automaton.AutomatonCode.Piles;
using Automaton.AutomatonCode.Powers;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace Automaton.AutomatonCode.Cards.Token;

[Pool(typeof(TokenCardPool))]
public class Terminator : AutomatonCardModel,
    IEncodable
{
    public Terminator() : base(1, CardType.Skill, CardRarity.Token, TargetType.Self)
    {
        WithCostUpgradeBy(-1);
        WithTip(StaticHoverTip.ReplayStatic);
        this.WithPower<TerminatorPower>(1, false);
    }


    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var a = AutomatonCmd.GetMax(Owner);
        var b = EncodePile.FunctionSequence.GetPile(Owner).Cards.Count;
        if (a - 1 != b) return;
        await CommonActions.ApplySelf<TerminatorPower>(ctx, this);
    }
}