using BaseLib.Abstracts;
using BaseLib.Utils;
using Collector.CollectorCode.Core;
using Collector.CollectorCode.CustomEnums;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace Collector.CollectorCode.Cards.Uncommon;


[Pool(typeof(CollectorCardPool))]
public class MisfortuneCookies : CollectorCardModel
{
    public MisfortuneCookies() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        WithKeyword(CardKeyword.Exhaust);
        WithTip(CollectorTip.Kindle);
        
        WithCalculatedVar("Cards", 0, Calc);
        WithCalculatedVar("Kindle", 0, 1,Calc,0,1);
    }

    private static decimal Calc(CardModel arg1, Creature? creature)
    {
        return creature?.Powers.Count(ShouldCountPower) ?? 0;
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var cards = (int)((CalculatedVar)DynamicVars["Cards"]).Calculate(cardPlay.Target);
        var kindle = (int)((CalculatedVar)DynamicVars["Kindle"]).Calculate(cardPlay.Target);
        await CardPileCmd.Draw(ctx, cards, Owner);
        await CollectorCmd.Kindle(ctx, Owner, kindle, this);
    }
    
    private static bool ShouldCountPower(PowerModel power)
    {
        return power.TypeForCurrentAmount == PowerType.Debuff && power is not ITemporaryPower;
    }
}