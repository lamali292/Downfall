using Automaton.AutomatonCode.Core;
using Automaton.AutomatonCode.Powers;
using BaseLib.Utils;
using Downfall.DownfallCode.CustomEnums;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Automaton.AutomatonCode.Cards.Uncommon;

[Pool(typeof(AutomatonCardPool))]
public class Repulsor : AutomatonCardModel
{
    public Repulsor() : base(2, CardType.Power, CardRarity.Uncommon, TargetType.None)
    {
        WithTip(DownfallTip.Status);
        WithPower<RepulsePower>(4, 1, false);
    }

    protected override Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
      => CommonActions.ApplySelf<RepulsePower>(ctx, this);
}