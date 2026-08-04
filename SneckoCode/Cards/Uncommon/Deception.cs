using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using Snecko.SneckoCode.Core;
using Snecko.SneckoCode.CustomEnums;

namespace Snecko.SneckoCode.Cards.Uncommon;

[Pool(typeof(SneckoCardPool))]
public class Deception : SneckoCardModel
{
    public Deception() : base(2, CardType.Skill, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        WithBlock(8, 3);
        WithKeyword(CardKeyword.Exhaust);
        this.WithTip<WeakPower>();
        this.WithTip<VulnerablePower>();
        WithTip(SneckoTip.Offclass);
        WithCalculatedVar("CalculatedPower", 1, Calc);
    }

    private static decimal Calc(CardModel card, Creature? arg2)
    {
        return CombatManager.Instance.History.CardPlaysFinished.Count(e =>
            e.Actor == card.Owner.Creature && e.HappenedThisTurn(card.CombatState) && SneckoCmd.IsOffclass(e.CardPlay.Card));
    }
    

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardBlock(this, cardPlay);
        if (cardPlay.Target == null) return;
        var amount = ((CalculatedVar)DynamicVars["CalculatedPower"]).Calculate(cardPlay.Target);
        await PowerCmd.Apply<WeakPower>(ctx, cardPlay.Target, amount, Owner.Creature, this);
        await PowerCmd.Apply<VulnerablePower>(ctx, cardPlay.Target, amount, Owner.Creature, this);
    }
}