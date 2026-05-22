using BaseLib.Utils;
using Hexaghost.HexaghostCode.Cards.Uncommon;
using Hexaghost.HexaghostCode.Core;
using Hexaghost.HexaghostCode.CustomEnums;
using Hexaghost.HexaghostCode.Events;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Hexaghost.HexaghostCode.Relics;

[Pool(typeof(HexaghostRelicPool))]
public class SneakyTeakwoodMatch : HexaghostRelicModel, IAfterGhostflameIgnited
{
    public SneakyTeakwoodMatch() : base(RelicRarity.Rare)
    {
        WithTip(HexaghostKeyword.Advance);
        WithTip(HexaghostKeyword.Retract);
    }


    private bool UsedThisTurn { get; set; }

    public async Task AfterGhostflameIgnited(PlayerChoiceContext ctx, Player player, GhostflameModel flame, int index)
    {
        if (player != Owner || UsedThisTurn) return;
        UsedThisTurn = true;
        Flash();
        Status = RelicStatus.Normal;
        var choices = new[] { HexaghostKeyword.Advance, HexaghostKeyword.Retract }
            .Select(f => FlareFlickChoice.Create(f, Owner))
            .ToList();
        var chosen = await CardSelectCmd.FromChooseACardScreen(ctx, choices, Owner);
        if (chosen is not FlareFlickChoice { Keyword : var keyword }) return;
        if (keyword == HexaghostKeyword.Advance)
            await HexaghostCmd.Advance(ctx, Owner, this);
        else if (keyword == HexaghostKeyword.Retract)
            await HexaghostCmd.Retract(ctx, Owner, this);
    }

    public override Task AfterSideTurnStart(CombatSide side, IReadOnlyList<Creature> participants,
        ICombatState combatState)
    {
        if (side != Owner.Creature.Side) return Task.CompletedTask;
        Status = RelicStatus.Active;
        UsedThisTurn = false;
        return Task.CompletedTask;
    }
}