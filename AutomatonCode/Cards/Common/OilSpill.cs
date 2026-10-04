using Automaton.AutomatonCode.Cards.Status;
using Automaton.AutomatonCode.Compile;
using Automaton.AutomatonCode.Core;
using Automaton.AutomatonCode.CustomEnums;
using Automaton.AutomatonCode.Encode;
using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Automaton.AutomatonCode.Cards.Common;

[Pool(typeof(AutomatonCardPool))]
public class OilSpill : AutomatonCardModel
{
    public OilSpill() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
        WithEncode<DamageEncode>();
        WithEncode<PoisonEncode>();
        WithCompile<ErrorToStashCompile>();
        WithDamage(4, 1);
        WithPower<PoisonPower>(4, 1);
        WithTip(AutomatonTip.Stash);
        WithTip<Error>();
    }

    protected override Artist Artist => Artist.Get<Opal>();

    protected override Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        return EncodeOutcome.EncodePlayEffect(this, ctx, cardPlay);
    }
}