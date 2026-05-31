using Automaton.AutomatonCode.Core;
using Automaton.AutomatonCode.Powers;
using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Cards;

namespace Automaton.AutomatonCode.Cards.Rare;

[Pool(typeof(AutomatonCardPool))]
public class Recreate : AutomatonCardModel
{
    public Recreate() : base(3, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
        this.WithPower<RecreatePower>(1, false);
        this.WithPower<RecreatePlusPower>(1, false);
        WithUpgradingCardTip<Fuel>();
    }

    protected override Artist Artist => Artist.Get<Opal>();


    protected override Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        return IsUpgraded
            ? CommonActions.ApplySelf<RecreatePlusPower>(ctx, this)
            : CommonActions.ApplySelf<RecreatePower>(ctx, this);
    }
}