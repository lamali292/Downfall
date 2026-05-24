using Automaton.AutomatonCode.Core;
using BaseLib.Utils;
using Downfall.DownfallCode.Extensions;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace Automaton.AutomatonCode.Cards.Rare;

[Pool(typeof(AutomatonCardPool))]
public class ProtoBeam : AutomatonCardModel
{
    public ProtoBeam() : base(2, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
    {
        WithDamage(5, 2);
        WithCalculatedVar("CalculatedHits", 0, Calc);
    }

    private static decimal Calc(CardModel card, Creature? arg2) => card.Owner.GetExhaust().Count;

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var exhaustCount =(int)((CalculatedVar)DynamicVars["CalculatedHits"]).Calculate(null);
        await CommonActions.CardAttack(this, cardPlay, exhaustCount).Execute(ctx);
    }
}