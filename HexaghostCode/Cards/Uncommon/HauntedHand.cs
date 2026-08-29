using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using Hexaghost.HexaghostCode.Core;
using Hexaghost.HexaghostCode.Interfaces;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Hexaghost.HexaghostCode.Cards.Uncommon;

[Pool(typeof(HexaghostCardPool))]
public class HauntedHand : HexaghostCardModel, IHasAfterlifeEffect
{
    public HauntedHand() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithAfterlife();
        WithBlock(5, 3);
    }

    protected override Artist Artist => Artist.Get<Opal>();

    public async Task AfterlifeEffect(PlayerChoiceContext ctx, CardPlay? cardPlay, bool wasExhausted,
        bool causedByEthereal)
    {
        while (CardPile.GetCards(Owner, PileType.Hand).Count() < 10)
        {
            var drawn = await CardPileCmd.Draw(ctx, Owner);
            if (wasExhausted && causedByEthereal) drawn?.GiveSingleTurnRetain();
            if (drawn == null || !drawn.Keywords.Contains(CardKeyword.Ethereal)) return;
        }
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardBlock(this, cardPlay);
        await AfterlifeEffect(ctx, cardPlay, false, false);
    }
}