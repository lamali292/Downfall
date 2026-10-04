using Automaton.AutomatonCode.Core;
using Automaton.AutomatonCode.Encode;
using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.ValueProps;

namespace Automaton.AutomatonCode.Cards.Token;

[Pool(typeof(TokenCardPool))]
public class Separator : AutomatonCardModel
{
    public Separator() : base(1, CardType.Attack, CardRarity.Token, TargetType.AnyEnemy)
    {
        WithEncode<DamageEncode>();
        WithEncode<MiddleDamageEncode>();
        WithDamage(6, 2);
        WithVars(new DamageVar("ExtraDamage", 6, DamageProps.card).WithUpgrade(2));
    }

    protected override Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        return EncodeOutcome.EncodePlayEffect(this, ctx, cardPlay);
    }
}