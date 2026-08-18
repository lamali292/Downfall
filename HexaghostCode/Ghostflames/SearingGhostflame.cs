using Downfall.DownfallCode.Powers;
using Hexaghost.HexaghostCode.Core;
using Hexaghost.HexaghostCode.DynamicVars;
using Hexaghost.HexaghostCode.Extensions;
using Hexaghost.HexaghostCode.Ghostflames.Intents;
using Hexaghost.HexaghostCode.Vfx;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;

namespace Hexaghost.HexaghostCode.Ghostflames;

public class SearingGhostflame : GhostflameModel
{
    protected override int IgnitionRequirement => 2;

    public override FireColor FireColor => FireColor.Yellow;

    public override AbstractIntent Intent => new MultiStatusIntent<SoulBurnPower>(
        () => DynamicVars.GhostflameSoulburn,
        2 * Repeat(GhostflameRepeatType.Soulburn)
    );

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<SoulBurnPower>()
    ];
    
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new GhostflameSoulburnVar(3)
    ];

    
    public override async Task OnIgnite(PlayerChoiceContext ctx)
    {
        if (!TryBeginIgnite()) return;
        var soulburn = DynamicVars.GhostflameSoulburn;
        var repeat = 2 * Repeat(GhostflameRepeatType.Soulburn);
        await RepeatOnTargets(ctx, repeat, GhostflameRepeatType.Soulburn,
            targets => PowerCmd.Apply<SoulBurnPower>(ctx, targets, soulburn, Owner.Creature, null));
    }

    protected override Task BeforeCardPlayed(PlayerChoiceContext ctx, CardPlay cardPlay)
        => TriggerOnCardType(ctx, cardPlay, CardType.Attack);

    public override bool AboutToIgnite(CardModel card)
    {
        return card.Type == CardType.Attack && IgnitionRequirement - IgnitionProgress <= 1;
    }
}