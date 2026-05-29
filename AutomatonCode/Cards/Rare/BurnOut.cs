using Automaton.AutomatonCode.Core;
using Automaton.AutomatonCode.CustomEnums;
using Automaton.AutomatonCode.Powers;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Downfall.DownfallCode.Artists;

namespace Automaton.AutomatonCode.Cards.Rare;

[Pool(typeof(AutomatonCardPool))]
public class BurnOut : AutomatonCardModel
{
    public BurnOut() : base(2, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
        this.WithPower<BurnOutPower>(9, 3, false);
        WithTip(AutomatonTip.Stash);
        WithTip(CardKeyword.Exhaust);
    }

    protected override Artist Artist => Artist.Get<Opal>();

    protected override Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        return CommonActions.ApplySelf<BurnOutPower>(ctx, this);
    }
}