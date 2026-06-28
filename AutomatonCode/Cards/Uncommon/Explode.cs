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
public class Explode : AutomatonCardModel, IEncodable<ExplodeEncode>
{
    public Explode() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.AllEnemies)
    {
        this.WithPower<ExplodePower>(2, false);
        this.WithTip<Burn>();
    }

    protected override Artist Artist => Artist.Get<Opal>();
    

    
    protected override Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        return CommonActions.ApplySelf<ExplodePower>(ctx, this);
    }
}