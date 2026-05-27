using Automaton.AutomatonCode.Core;
using Automaton.AutomatonCode.CustomEnums;
using Automaton.AutomatonCode.Powers;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Automaton.AutomatonCode.Cards.Rare;

[Pool(typeof(AutomatonCardPool))]
public class Library : AutomatonCardModel
{
    public Library() : base(3, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
        this.WithPower<LibraryPower>(1, false);
        WithTip(AutomatonTip.Encode);
        WithEnergyTip();
        WithCostUpgradeBy(-1);
    }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<LibraryPower>(ctx, this);
    }
}