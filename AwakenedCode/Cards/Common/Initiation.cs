using Awakened.AwakenedCode.Cards.Token;
using Awakened.AwakenedCode.Core;
using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using Downfall.DownfallCode.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Awakened.AwakenedCode.Cards.Common;

[Pool(typeof(AwakenedCardPool))]
public class Initiation : AwakenedCardModel
{
    public Initiation() : base(2, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        WithBlock(11, 3);
        this.WithTip<Ceremony>();
    }

    protected override Artist Artist => Artist.Get<Opal>();

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardBlock(this, cardPlay);
        await DownfallCardCmd.GiveCard<Ceremony>(Owner, PileType.Hand);
    }
}