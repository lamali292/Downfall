using BaseLib.Utils;
using Collector.CollectorCode.Core;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Collector.CollectorCode.Cards.Common;

[Pool(typeof(CollectorCardPool))]
public class Wildfire : CollectorCardModel
{
    public Wildfire() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
        WithCalculatedDamage(0, 4, DamageCalc, ValueProp.Move, 0, 2);
    }

    protected override Artist Artist => Artist.Get<Opal>();

    private static decimal DamageCalc(CardModel card, Creature? creature)
    {
        return creature?.Powers.Count(e => e.Type == PowerType.Debuff) ?? 0;
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
    }
}