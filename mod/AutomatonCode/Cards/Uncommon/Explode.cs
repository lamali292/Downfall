using Automaton.AutomatonCode.Compile;
using Automaton.AutomatonCode.Core;
using Automaton.AutomatonCode.Encode;
using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using Downfall.DownfallCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Cards;

namespace Automaton.AutomatonCode.Cards.Uncommon;

[Pool(typeof(AutomatonCardPool))]
public class Explode : AutomatonCardModel
{
    public Explode() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.AllEnemies)
    {
        WithEncode<SoulburnEncode>();
        WithCompile<BurnToDrawCompile>();
        WithCards(1);
        WithPower<SoulBurnPower>(15, 5);
        WithTip<Burn>();
    }

    protected override Artist Artist => Artist.Get<Opal>();

    protected override Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        return EncodeOutcome.EncodePlayEffect(this, ctx, cardPlay);
    }
}