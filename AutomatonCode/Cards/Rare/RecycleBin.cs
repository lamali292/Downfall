using Automaton.AutomatonCode.Core;
using Automaton.AutomatonCode.Powers;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Automaton.AutomatonCode.Cards.Rare;

[Pool(typeof(AutomatonCardPool))]
public class RecycleBin : AutomatonCardModel
{
    public RecycleBin() : base(1, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
        WithPower<RecycleBinPower>(4, 1);
    }

    protected override Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
        => CommonActions.ApplySelf<RecycleBinPower>(ctx, this);
}