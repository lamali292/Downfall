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
        WithTip(SlimeBossTip.Consume);
    }

    private static decimal Calc(CardModel card, Creature? _)
    {
        return card.Owner.Hand.Count(e => e.Type == CardType.Status);
    }

    public async Task ConsumeEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CardPileCmd.Draw(ctx, DynamicVars.Cards.BaseValue, Owner);
        var a = ((CalculatedVar)DynamicVars["Cards"]).Calculate(cardPlay.Target);
        await CommonActions.ApplySelf<DrawCardsNextTurnPower>(ctx, this, a);
    }
}
