using Automaton.AutomatonCode.Core;
using Automaton.AutomatonCode.Encode;
using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Automaton.AutomatonCode.Cards.Rare;

[Pool(typeof(AutomatonCardPool))]
public class DigitalCarnage : AutomatonCardModel
{
    public DigitalCarnage() : base(2, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
    {
        WithEncode<DamageEncode>();
        WithKeyword(CardKeyword.Ethereal);
        WithDamage(20, 8);
    }

    protected override Artist Artist => Artist.Get<Opal>();

    protected override Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        return EncodeOutcome.EncodePlayEffect(this, ctx, cardPlay);
    }
}