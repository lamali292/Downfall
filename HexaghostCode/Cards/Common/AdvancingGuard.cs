using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using Hexaghost.HexaghostCode.Core;
using Hexaghost.HexaghostCode.CustomEnums;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Hexaghost.HexaghostCode.Cards.Common;

[Pool(typeof(HexaghostCardPool))]
public class AdvancingGuard : HexaghostCardModel
{
    public AdvancingGuard() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        WithKeyword(HexaghostKeyword.Advance);
        WithBlock(8, 3);
    }

    protected override Artist Artist => Artist.Get<Inmo>();

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardBlock(this, cardPlay);
    }
}