using Automaton.AutomatonCode.Cards.Token;
using Automaton.AutomatonCode.Core;
using Automaton.AutomatonCode.Powers;
using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Automaton.AutomatonCode.Cards.Uncommon;

[Pool(typeof(AutomatonCardPool))]
public class InfiniteBeams : AutomatonCardModel
{
    public InfiniteBeams() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
    {
        WithKeyword(CardKeyword.Innate, UpgradeType.Add);
        this.WithTip<MinorBeam>();
        this.WithPower<InfiniteBeamsPower>(1, false);
    }

    protected override Artist Artist => Artist.Get<Opal>();

    protected override Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        return CommonActions.ApplySelf<InfiniteBeamsPower>(ctx, this);
    }
}