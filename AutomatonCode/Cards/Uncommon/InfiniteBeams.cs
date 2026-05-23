using Automaton.AutomatonCode.Cards.Token;
using Automaton.AutomatonCode.Core;
using Automaton.AutomatonCode.Powers;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;

namespace Automaton.AutomatonCode.Cards.Uncommon;

[Pool(typeof(AutomatonCardPool))]
public class InfiniteBeams : AutomatonCardModel
{
    public InfiniteBeams() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.None)
    {
        WithKeyword(CardKeyword.Innate, UpgradeType.Add);
        WithTip(typeof(MinorBeam));
        WithPower<InfiniteBeamsPower>(1, false);
    }

    protected override Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
        => CommonActions.ApplySelf<InfiniteBeamsPower>(ctx, this);
}