using BaseLib.Cards.Variables;
using BaseLib.Utils;
using Hexaghost.HexaghostCode.Core;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Hexaghost.HexaghostCode.Cards.Rare;

[Pool(typeof(HexaghostCardPool))]
public class UnleashSpirits : HexaghostCardModel
{
    public UnleashSpirits() : base(2, CardType.Attack, CardRarity.Rare, TargetType.RandomEnemy)
    {
        WithDamage(10, 3);
        WithTip(CardKeyword.Exhaust);
        WithCalculatedVar("Repeat", 1, Calc);
    }

    private static decimal Calc(CardModel card, Creature? target)
    {
        var combatState = card.CombatState;
        if (combatState == null) return 0;
        return CombatManager.Instance.History.Entries.OfType<CardExhaustedEntry>().Count(e =>
            e.RoundNumber == combatState.RoundNumber - 1 && e.Actor == card.Owner.Creature);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var repeat = ((CustomCalculatedVar)DynamicVars["Repeat"]).Calculate(null);
        await CommonActions.CardAttack(this, cardPlay, (int)repeat).Execute(ctx);
    }
}