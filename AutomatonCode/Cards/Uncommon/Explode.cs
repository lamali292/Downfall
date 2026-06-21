using Automaton.AutomatonCode.Core;
using Automaton.AutomatonCode.Interfaces;
using Automaton.AutomatonCode.Powers;
using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using Downfall.DownfallCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Cards;

namespace Automaton.AutomatonCode.Cards.Uncommon;

[Pool(typeof(AutomatonCardPool))]
public class Explode : AutomatonCardModel, IEncodable
{
    public Explode() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.AllEnemies)
    {
        this.WithPower<ExplodePower>(2, false);
        this.WithTip<Burn>();
        WithPower<SoulBurnPower>(15, 5);
    }

    protected override Artist Artist => Artist.Get<Opal>();

    public Task PlayEncodableEffect(PlayerChoiceContext ctx, CardPlay cardPlay, EncodeContext encodeContext)
    {
        return CommonActions.Apply<SoulBurnPower>(ctx, this, cardPlay);
    }

    protected override Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        return CommonActions.ApplySelf<ExplodePower>(ctx, this);
    }
}