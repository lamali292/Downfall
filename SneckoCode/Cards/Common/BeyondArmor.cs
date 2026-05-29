using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Snecko.SneckoCode.Core;
using Snecko.SneckoCode.Extensions;
using Snecko.SneckoCode.Interfaces;

namespace Snecko.SneckoCode.Cards.Common;

[Pool(typeof(SneckoCardPool))]
public class BeyondArmor : SneckoCardModel, IHasGift
{
    public BeyondArmor() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        WithBlock(5, 3);
        WithCards(2);
        this.WithGift(new Gift
        {
            Rarity = CardRarity.Common
        });
    }

    public Gift? Gift { get; set; }


    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardBlock(this, cardPlay);
        var cards = Owner.GetDraw()
            .Where(c => SneckoCmd.IsOffclass(this, c))
            .TakeRandom(DynamicVars.Cards.IntValue, Owner.RunState.Rng.CombatCardSelection);
        await CardPileCmd.Add(cards, PileType.Hand);
    }
}