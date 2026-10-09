using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using Hexaghost.HexaghostCode.Core;
using Hexaghost.HexaghostCode.CustomEnums;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Hexaghost.HexaghostCode.Cards.Uncommon;

[Pool(typeof(HexaghostCardPool))]
public class Divider : HexaghostCardModel
{
    public Divider() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        WithDamage(4, 2);
        WithTip(HexaghostTip.Ignite);
        WithCalculatedVar("Hits", 0, Calc);
    }

    private static decimal Calc(CardModel card, Creature? _)
    {
        return HexaghostCmd.GetIgnitedCount(card.Owner);
    }

    protected override Artist Artist => Artist.Get<CartesianCanvas>();

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var count =  (int)DynamicVars["Hits"].Calculate(cardPlay.Target);
        if (count == 0) return;
        await CommonActions.CardAttack(this, cardPlay, count).Execute(ctx);
    }
}