using Awakened.AwakenedCode.Core;
using Awakened.AwakenedCode.Extensions;
using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Awakened.AwakenedCode.Cards.Common;

[Pool(typeof(AwakenedCardPool))]
public class Envision : AwakenedCardModel
{
    public Envision() : base(0, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        WithBlock(4, 3);
        this.WithConjure();
    }

    protected override Artist Artist => Artist.Get<Opal>();

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardBlock(this, cardPlay);
        var card = await AwakenedCmd.Conjure(Owner);
        if (card == null) return;
        await CardPileCmd.Add(card, PileType.Draw, CardPilePosition.Top);
    }
}