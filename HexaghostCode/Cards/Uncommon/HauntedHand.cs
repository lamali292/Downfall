using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using Hexaghost.HexaghostCode.Core;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Hexaghost.HexaghostCode.Cards.Uncommon;

[Pool(typeof(HexaghostCardPool))]
public class HauntedHand : HexaghostCardModel
{
    public HauntedHand() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithBlock(5, 3);
        WithTip(CardKeyword.Ethereal);
    }

    protected override Artist Artist => Artist.Get<Opal>();

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardBlock(this, cardPlay);

        while (CardPile.GetCards(Owner, PileType.Hand).Count() < 10)
        {
            var drawn = await CardPileCmd.Draw(ctx, Owner);
            if (drawn == null || !drawn.Keywords.Contains(CardKeyword.Ethereal)) return;
        }
    }
}