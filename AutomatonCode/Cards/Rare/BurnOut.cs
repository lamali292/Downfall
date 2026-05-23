using Automaton.AutomatonCode.Core;
using Automaton.AutomatonCode.CustomEnums;
using Automaton.AutomatonCode.Powers;
using BaseLib.Utils;
using Downfall.DownfallCode.CustomEnums;
using Downfall.DownfallCode.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Automaton.AutomatonCode.Cards.Rare;

[Pool(typeof(AutomatonCardPool))]
public class BurnOut : AutomatonCardModel
{
    public BurnOut() : base(2, CardType.Power, CardRarity.Rare, TargetType.None)
    {
        WithPower<BurnOutPower>(9, 3);
        WithTip(AutomatonTip.Stash);
        WithTip(CardKeyword.Exhaust);
    }

    protected override Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
        => CommonActions.ApplySelf<BurnOutPower>(ctx, this);
}