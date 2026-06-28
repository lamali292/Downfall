using Automaton.AutomatonCode.Core;
using Automaton.AutomatonCode.Encode;
using Automaton.AutomatonCode.Interfaces;
using Automaton.AutomatonCode.Powers;
using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Cards;

namespace Automaton.AutomatonCode.Cards.Uncommon;

[Pool(typeof(AutomatonCardPool))]
public class DazingPulse : AutomatonCardModel, IEncodable<DazingPulseEncode>
{
    public DazingPulse() : base(2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        this.WithPower<DazingPulsePower>(2, false);
        this.WithTip<Dazed>();
    }

    protected override Artist Artist => Artist.Get<Opal>();
    
    protected override Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        return CommonActions.ApplySelf<DazingPulsePower>(ctx, this);
    }
}