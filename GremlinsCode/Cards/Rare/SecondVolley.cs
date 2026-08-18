using BaseLib.Utils;
using Downfall.DownfallCode.Commands;
using Gremlins.GremlinsCode.Core;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;

namespace Gremlins.GremlinsCode.Cards.Rare;

[Pool(typeof(GremlinsCardPool))]
public class SecondVolley : GremlinsCardModel
{
    public SecondVolley() : base(2, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
    {
        WithCalculatedVar("CardsPlayed", 0, Calc);
        WithUpgradingCardTip<Shiv>();
        WithDamage(4, 2);
    }

    private static decimal Calc(CardModel card, Creature? _)
    {
        var cardsPlayed = CombatManager.Instance.History.CardPlaysFinished.Count(e =>
            e.HappenedThisTurn(card.CombatState) && e.Actor == card.Owner.Creature);
        var maxDraw = CardPile.MaxCardsInHand - card.Owner.Hand.Count(e => e != card);
        return Math.Min(maxDraw, cardsPlayed);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
        if (Owner.PlayerCombatState == null) return;
        var cardsPlayed = ((CalculatedVar)DynamicVars["CardsPlayed"]).Calculate(null);
        await DownfallCardCmd.GiveCards<Shiv>(Owner, PileType.Hand, cardsPlayed, upgraded: IsUpgraded);
    }
}