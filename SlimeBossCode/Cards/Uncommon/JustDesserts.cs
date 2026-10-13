using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using SlimeBoss.SlimeBossCode.Core;
using SlimeBoss.SlimeBossCode.CustomEnums;

namespace SlimeBoss.SlimeBossCode.Cards.Uncommon;

[Pool(typeof(SlimeBossCardPool))]
public class JustDesserts : SlimeBossCardModel
{
    public JustDesserts() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AllEnemies)
    {
        WithDamage(8, 4);
        WithCalculatedVar("Cards", 0, Calc);
        WithKeyword(CardKeyword.Exhaust);
    }

    private static decimal Calc(CardModel card, Creature? _)
    {
        return card.Owner.Hand.Count(e => e.Type == CardType.Status);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
        var cards = ((CalculatedVar)DynamicVars["Cards"]).Calculate(cardPlay.Target);
        if (cards > 0)
            await CommonActions.ApplySelf<DrawCardsNextTurnPower>(ctx, this, cards);
    }
}
