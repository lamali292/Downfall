using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using SlimeBoss.SlimeBossCode.Core;
using SlimeBoss.SlimeBossCode.Extensions;
using SlimeBoss.SlimeBossCode.CustomEnums;

namespace SlimeBoss.SlimeBossCode.Cards.Uncommon;

[Pool(typeof(SlimeBossCardPool))]
public class DivideAndConquer : SlimeBossCardModel
{
    public DivideAndConquer() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        WithTip(SlimeBossTip.Split);
        WithDamage(4, 2);
        WithCalculatedVar("Repeat", 1, Calc);
    }

    protected override Artist Artist => Artist.Get<Opal>();

    private static decimal Calc(CardModel card, Creature? _)
    {
        return card.Owner.SlimeCount;
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var hits = (int)((CalculatedVar)DynamicVars["Repeat"]).Calculate(null);
        await CommonActions.CardAttack(this, cardPlay, hits).Execute(ctx);
    }
}
