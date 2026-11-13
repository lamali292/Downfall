using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using Hexaghost.HexaghostCode.Core;
using Hexaghost.HexaghostCode.Interfaces;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Hexaghost.HexaghostCode.Cards.Multiplayer;

[Pool(typeof(HexaghostCardPool))]
public class Levitate : HexaghostCardModel, IHasAfterlifeEffect
{
    public Levitate() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.AnyAlly)
    {
        WithAfterlife();
        WithBlock(9, 3);
    }

    protected override Artist Artist => Artist.Get<Chimedragon>();


    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;

    public Task AfterlifeEffect(PlayerChoiceContext ctx, CardPlay? cardPlay, bool wasExhausted, bool causedByEthereal)
    {
        var target = cardPlay?.Target ?? Owner.RandomOtherTeammate?.Creature;
        return target == null ? Task.CompletedTask : CreatureCmd.GainBlock(target, DynamicVars.Block, cardPlay);
    }

    protected override Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        return AfterlifeEffect(ctx, cardPlay, false, false);
    }
}