using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Cards.CardModels;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Downfall.Code.Cards.Awakened.Common;

[Pool(typeof(AwakenedCardPool))]
public class Clutch : AwakenedCardModel
{
    public Clutch() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
        WithDamage(8, 3);
    }

    protected override bool ShouldGlowRedInternal => Has0CostInDraw;

    private bool Has0CostInDraw
    {
        get
        {
            return !PileType.Draw.GetPile(Owner)
                .Cards.Any(c => c.EnergyCost is { Canonical: 0, CostsX: false });
        }
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
        var card = PileType.Draw.GetPile(Owner)
            .Cards.FirstOrDefault(c => c.EnergyCost is { Canonical: 0, CostsX: false });
        if (card == null) return;
        await CardPileCmd.Add(card, PileType.Hand);
    }
}