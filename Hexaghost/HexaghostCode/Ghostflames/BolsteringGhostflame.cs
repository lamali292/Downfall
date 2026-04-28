using Hexaghost.HexaghostCode.Core;
using Hexaghost.HexaghostCode.Events;
using Hexaghost.HexaghostCode.Ghostflames.Intents;
using Hexaghost.HexaghostCode.Vfx;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Multiplayer;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.ValueProps;

namespace Hexaghost.HexaghostCode.Ghostflames;

public class BolsteringGhostflame : GhostflameModel
{
    public override AbstractIntent Intent => new BolsteringIntent();
    protected override int IgnitionRequirement => 1;

    public override FireColor FireColor => FireColor.Blue;

    public override async Task OnIgnite(PlayerChoiceContext ctx)
    {
        if (Owner.Creature.CombatState == null) return;
        
        SfxCmd.Play("event:/sfx/characters/attack_fire");
        
        var repeat = Repeat(GhostflameRepeatType.Block);
        for (var i = 0; i < repeat; i++)
        {
            await CreatureCmd.GainBlock(Owner.Creature, 4 + Intensity, ValueProp.Move | ValueProp.Unpowered, null);
        }
        
        await PowerCmd.Apply<StrengthPower>(ctx, Owner.Creature, 1, Owner.Creature, null);
    }

    public override async Task BeforeCardPlayed(CardPlay cardPlay)
    {
        if (!IsActive || cardPlay.Card.Owner != Owner ||
            LocalContext.NetId == null) return;
        var shouldCount = HexaghostHook.GhostflameConditionOverwrites(CombatState, Owner, this, cardPlay);
        if (!(cardPlay.Card.Type == CardType.Power || shouldCount)) return;
    
        if (!TryProgress()) return;
        var ctx = new HookPlayerChoiceContext(
            Owner,
            LocalContext.NetId.Value,
            GameActionType.Combat);
        var task = Ignite(ctx);
        await ctx.AssignTaskAndWaitForPauseOrCompletion(task);
    }
}