using BaseLib.Utils;
using Downfall.DownfallCode.Commands;
using Downfall.DownfallCode.Powers;
using Hexaghost.HexaghostCode.Core;
using Hexaghost.HexaghostCode.Extensions;
using Hexaghost.HexaghostCode.Interfaces;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Hexaghost.HexaghostCode.Cards.Multiplayer;

[Pool(typeof(HexaghostCardPool))]
public class CircleFlame : HexaghostCardModel, IHasAfterlifeEffect
{
    public CircleFlame() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        this.WithAfterlife();
        WithPower<SoulBurnPower>(14, 4);
        WithTip(CardKeyword.Exhaust);
    }

    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;

    protected override Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        return AfterlifeEffect(ctx, cardPlay, false, false);
    }


    public async Task AfterlifeEffect(PlayerChoiceContext ctx, CardPlay? cardPlay, bool wasExhausted, bool causedByEthereal)
    {
        var target = cardPlay?.Target ?? CombatState?.RunState.Rng.CombatTargets.NextItem(CombatState.HittableEnemies);
        await MyCommonActions.Apply<SoulBurnPower>(ctx, this, target);

        if (!wasExhausted) return;
        var teammates = CombatState?.GetTeammatesOf(Owner.Creature)
            .Where(e => e != Owner.Creature && e is { IsPlayer: true, IsAlive: true }) ?? [];
        var player = CombatState?.RunState.Rng.CombatTargets.NextItem(teammates)?.Player;
        if (player == null) return;
        // TODO: use CreateCloneForPlayer on main / beta merge
        var clone = CreateClone();
        clone._owner = player;
        var a = await CardPileCmd.AddGeneratedCardToCombat(clone, PileType.Discard, Owner);
        CardCmd.PreviewCardPileAdd(a, 0.5f);
    }
}