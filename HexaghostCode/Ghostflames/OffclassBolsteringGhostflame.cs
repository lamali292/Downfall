using Downfall.DownfallCode.Commands;
using Downfall.DownfallCode.CustomEnums;
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
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.ValueProps;

namespace Hexaghost.HexaghostCode.Ghostflames;

public class OffclassBolsteringGhostflame : GhostflameModel
{
    public override AbstractIntent Intent => new BolsteringIntent(
        () => DynamicVars.GhostflameBlock);
    protected override int IgnitionRequirement => 1;

    public override FireColor FireColor => FireColor.Blue;
    public override bool IsOffclass => true;

    
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new GhostflameBlockVar( 3),
        new PowerVar<StrengthPower>(1)
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.Static(DownfallTip.Offclass),
        HoverTipFactory.Static(StaticHoverTip.Block),
        HoverTipFactory.FromPower<StrengthPower>()
    ];

    public override async Task OnIgnite(PlayerChoiceContext ctx)
    {
        if (!TryBeginIgnite()) return;

        var block = DynamicVars.GhostflameBlock;
        var repeat = Repeat(GhostflameRepeatType.Block);
        for (var i = 0; i < repeat; i++)
        {
            await CreatureCmd.GainBlock(Owner.Creature, block, BlockProps.nonCardUnpowered, null);
            await MyCommonActions.ApplySelf<StrengthPower>(ctx, this);
        }
    }

    protected override Task BeforeCardPlayed(PlayerChoiceContext ctx, CardPlay cardPlay)
        => TriggerOnCardType(ctx, cardPlay, CardType.Power, DownfallCmd.IsOffclass);
    
    public override bool AboutToIgnite(CardModel card)
    {
        return card.Type == CardType.Power && DownfallCmd.IsOffclass(card) && IgnitionRequirement - IgnitionProgress <= 1;
    }
}