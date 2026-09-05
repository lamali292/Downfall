using BaseLib.Utils;
using Collector.CollectorCode.Core;
using Downfall.DownfallCode.Artists;
using Downfall.DownfallCode.Compatibility;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Collector.CollectorCode.Cards.Uncommon;

[Pool(typeof(CollectorCardPool))]
public class Extricate : CollectorCardModel
{
    public Extricate() : base(2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        WithDamage(15, 5);
        WithTip(CardKeyword.Exhaust);
    }

    protected override Artist Artist => Artist.Get<Opal>();

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var cardsToExhaust = Owner.Hand.Where(c => c.Type != CardType.Attack).ToList();
        foreach (var card in cardsToExhaust) await CardCmdCompatibility.Exhaust(ctx, card);
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
    }
}