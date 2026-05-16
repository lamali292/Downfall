using BaseLib.Utils;
using Collector.CollectorCode.Core;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Collector.CollectorCode.Cards.Uncommon;

[Pool(typeof(CollectorCardPool))]
public class WhirlingFlame : CollectorCardModel
{
    public WhirlingFlame() : base(2, CardType.Attack, CardRarity.Uncommon, TargetType.AllEnemies)
    {
        WithDamage(15, 4);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
        var a = await CommonActions.SelectSingleCard(this, SelectionScreenPrompt, ctx, PileType.Discard);
        if (a == null) return;
        await CardPileCmd.Add(a, PileType.Draw, CardPilePosition.Top);
    }
}