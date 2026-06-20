using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using Hexaghost.HexaghostCode.Core;
using Hexaghost.HexaghostCode.CustomEnums;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Hexaghost.HexaghostCode.Cards.Common;

[Pool(typeof(HexaghostCardPool))]
public class DevourFlame : HexaghostCardModel
{
    public DevourFlame() : base(0, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        WithBlock(9, 3);
        WithTip(HexaghostKeyword.Retract);
    }

    protected override Artist Artist => Artist.Get<Inmo>();

    protected override bool ShouldGlowGoldInternal => HexaghostCmd.IsPreviousIgnited(Owner);

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (!HexaghostCmd.IsPreviousIgnited(Owner)) return;
        await HexaghostCmd.Retract(ctx, Owner, this);
        await CommonActions.CardBlock(this, cardPlay);
    }
}