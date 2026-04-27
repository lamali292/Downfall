using BaseLib.Utils;
using Hexaghost.HexaghostCode.Core;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Hexaghost.HexaghostCode.Cards.Common;

[Pool(typeof(HexaghostCardPool))]
public class DevourFlame : HexaghostCardModel
{
    public DevourFlame() : base(0, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        WithBlock(9, 3);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (!HexaghostCmd.IsPreviousIgnited(Owner)) return;
        await HexaghostCmd.Retract(ctx, Owner, this);
        await CommonActions.CardBlock(this, cardPlay);
    }
}