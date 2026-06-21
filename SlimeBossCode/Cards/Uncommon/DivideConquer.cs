using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using SlimeBoss.SlimeBossCode.Core;

namespace SlimeBoss.SlimeBossCode.Cards.Uncommon;

[Pool(typeof(SlimeBossCardPool))]
public class DivideConquer : SlimeBossCardModel
{
    public DivideConquer() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.RandomEnemy)
    {
        WithCalculatedVar("Repeat", 0, Calc);
        WithDamage(10, 5);
        WithKeyword(CardKeyword.Exhaust);
    }

    protected override Artist Artist => Artist.Get<Opal>();

    private static decimal Calc(CardModel card, Creature? _)
    {
        return SlimeQueue.GetCount(card.Owner);
    }


    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var hits = (int)((CalculatedVar)DynamicVars["Repeat"]).Calculate(null);
        await CommonActions.CardAttack(this, cardPlay, hits).Execute(ctx);
        await SlimeBossCmd.AbsorbAll(ctx, this);
    }
}