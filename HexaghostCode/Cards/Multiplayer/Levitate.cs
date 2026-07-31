using BaseLib.Utils;
using Hexaghost.HexaghostCode.Core;
using Hexaghost.HexaghostCode.Extensions;
using Hexaghost.HexaghostCode.Interfaces;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Hexaghost.HexaghostCode.Cards.Multiplayer;

[Pool(typeof(HexaghostCardPool))]
public class Levitate : HexaghostCardModel, IHasAfterlifeEffect
{
    public Levitate()  : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.AnyAlly)
    {
        this.WithAfterlife();
        WithBlock(9, 3);
    }
    
    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;

    protected override Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        return AfterlifeEffect(ctx, cardPlay, false, false);
    }

    public Task AfterlifeEffect(PlayerChoiceContext ctx, CardPlay? cardPlay, bool wasExhausted, bool causedByEthereal)
    {
        var target = cardPlay?.Target ?? CombatState?.RunState.Rng.CombatTargets.NextItem(CombatState.GetTeammatesOf(Owner.Creature).Where(e => e != Owner.Creature && e is { IsPlayer: true, IsAlive: true }));
        return target == null ? Task.CompletedTask : CreatureCmd.GainBlock(target, DynamicVars.Block, cardPlay);
    }
}