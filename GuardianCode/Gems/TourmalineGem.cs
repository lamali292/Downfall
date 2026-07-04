using BaseLib.Abstracts;
using Downfall.DownfallCode.Powers;
using Godot;
using Guardian.GuardianCode.Core;
using Guardian.GuardianCode.DynamicVars;
using Guardian.GuardianCode.Events;
using Guardian.GuardianCode.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Guardian.GuardianCode.Gems;

public class TourmalineGem : GemModel
{
    public override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<ThornsPower>()];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new GemVar(4)];
    public override Color GemColor => new(0x06BE7BFF);
    public override CardRarity Rarity => CardRarity.Common;

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay? cardPlay)
    {
        var effect = GuardianHook.ModifyGemEffect(CombatState, this, DynamicVars.Gem().BaseValue, Card);
        await PowerCmd.Apply<TourmalineGemPower>(ctx, Player.Creature, effect, Player.Creature, null);
    }
}

public class TourmalineGemPower : CustomTemporaryPowerModelWrapper<TourmalineGem, ThornsPower>
{
    public override LocString Title => OriginModel is GemModel gem ? gem.Title : base.Title;
    protected override IEnumerable<IHoverTip> ExtraHoverTips => OriginModel is GemModel gem ? gem.HoverTips : base.ExtraHoverTips;
    protected override bool UntilEndOfOtherSideTurn => true;
}