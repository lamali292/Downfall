using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using Hexaghost.HexaghostCode.Core;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Hexaghost.HexaghostCode.Cards.Basic;

[Pool(typeof(HexaghostCardPool))]
public class Kindle : HexaghostCardModel
{
    public Kindle() : base(1, CardType.Skill, CardRarity.Basic, TargetType.Self)
    {
        WithBlock(3, 3);
    }

    protected override Artist Artist => Artist.Get<AlexMdle>();


    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardBlock(this, cardPlay);
        await HexaghostCmd.Ignite(ctx, Owner);
    }
}