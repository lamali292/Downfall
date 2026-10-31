using BaseLib.Utils;
using Collector.CollectorCode.Cards.Token;
using Collector.CollectorCode.Core;
using Downfall.DownfallCode.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
namespace Collector.CollectorCode.Cards.Common;

[Pool(typeof(CollectorCardPool))]
public class CastIron : CollectorCardModel
{
    public CastIron() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        WithDamage(4, 2);
        WithKeyword(CardKeyword.Exhaust);
        WithCalculatedVar("KindleCalc", 0, 1, Calc, 0, 1);
        WithKindle(2, 1); // only for the tip
    }

    private static decimal Calc(CardModel card, Creature? arg2)
    {
        return card.Owner.Hand.Count(e => e.Type == CardType.Status);
    }
    
    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var count = Owner.Hand.Count(e => e.Type == CardType.Status);
        var kindle = (int)((CalculatedVar)DynamicVars["KindleCalc"]).Calculate(null);
        await CommonActions.CardAttack(this, cardPlay, count).Execute(ctx);
        if (kindle > 0)
            await TorchheadCmd.Kindle(ctx, Owner, kindle, this);
    }

}