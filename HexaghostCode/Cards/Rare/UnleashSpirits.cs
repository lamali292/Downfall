using BaseLib.Cards.Variables;
using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using Hexaghost.HexaghostCode.Core;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Hexaghost.HexaghostCode.Cards.Rare;

[Pool(typeof(HexaghostCardPool))]
public class UnleashSpirits : HexaghostCardModel
{
    public UnleashSpirits() : base(1, CardType.Attack, CardRarity.Rare, TargetType.RandomEnemy)
    {
        WithDamage(6, 2);
        WithTip(CardKeyword.Ethereal);
        WithTip(CardKeyword.Exhaust);
        WithCalculatedVar("Repeat", 1, Calc);
    }

    protected override Artist Artist => Artist.Get<Inmo>();

    private static decimal Calc(CardModel card, Creature? target)
    {
        return card.Owner.ExhaustPile.Count(e => e.Keywords.Contains(CardKeyword.Ethereal));
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var repeat = ((CustomCalculatedVar)DynamicVars["Repeat"]).Calculate(null);
        var scale = 0.8f;
        await CommonActions.CardAttack(this, cardPlay, (int)repeat).BeforeDamage(async () =>
        {
            await HexaghostCmd.SoulburnEffect(cardPlay.Target, scale);
            scale += 0.1f;
        }).Execute(ctx);
    }
}