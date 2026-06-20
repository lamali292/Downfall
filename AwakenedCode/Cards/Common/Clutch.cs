using Awakened.AwakenedCode.Core;
using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Awakened.AwakenedCode.Cards.Common;

[Pool(typeof(AwakenedCardPool))]
public class Clutch : AwakenedCardModel
{
    public Clutch() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
        WithDamage(8, 3);
        WithEnergyTip();
    }

    protected override Artist Artist => Artist.Get<Opal>();

    protected override bool ShouldGlowGoldInternal => Has0CostInDraw;

    private bool Has0CostInDraw
    {
        get
        {
            return !PileType.Draw.GetPile(Owner)
                .Cards.Any(c => c.EnergyCost is { Canonical: 0, CostsX: false });
        }
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
        var card = PileType.Draw.GetPile(Owner)
            .Cards.FirstOrDefault(c => c.EnergyCost is { Canonical: 0, CostsX: false });
        if (card == null) return;
        await CardPileCmd.Add(card, PileType.Hand);
    }
}