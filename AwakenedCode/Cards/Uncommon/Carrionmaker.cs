using Awakened.AwakenedCode.Core;
using Awakened.AwakenedCode.Interfaces;
using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace Awakened.AwakenedCode.Cards.Uncommon;

[Pool(typeof(AwakenedCardPool))]
public class Carrionmaker : AwakenedCardModel
{
    public Carrionmaker() : base(2, CardType.Attack, CardRarity.Uncommon, TargetType.RandomEnemy)
    {
        WithDamage(9, 3);
        WithCalculatedVar("Hits", 1, Calc);
    }

    private static decimal Calc(CardModel card, Creature? arg2)
    {
        return CombatManager.Instance.History.CardPlaysStarted.Count(s =>
            s.CardPlay.Card.Owner.Creature == s.Actor && s.HappenedThisTurn(card.CombatState) && s.CardPlay.Card is ISpell);
    }

    protected override Artist Artist => Artist.Get<Opal>();

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var hits = (int)((CalculatedVar)DynamicVars["Hits"]).Calculate(cardPlay.Target);
        await CommonActions.CardAttack(this, cardPlay, hits).Execute(ctx);
    }
}