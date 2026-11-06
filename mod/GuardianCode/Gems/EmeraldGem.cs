using BaseLib.Abstracts;
using Godot;
using Guardian.GuardianCode.Core;
using Guardian.GuardianCode.DynamicVars;
using Guardian.GuardianCode.Events;
using Guardian.GuardianCode.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Guardian.GuardianCode.Gems;

public class EmeraldGem : GemModel
{
    public override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<DexterityPower>()];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new GemVar(2)];
    public override Color GemColor => new(0x319028FF);
    public override CardRarity Rarity => CardRarity.Uncommon;

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay? cardPlay,
        IEnumerable<Player> targetPlayers)
    {
        var owner = Player.Creature;
        var effect = GuardianHook.ModifyGemEffect(CombatState, this, DynamicVars.Gem.BaseValue, Card);
        foreach (var player in targetPlayers)
            await PowerCmd.Apply<EmeraldGemPower>(ctx, player.Creature, effect, owner, Card);
    }
}

public class EmeraldGemPower : CustomTemporaryPowerModelWrapper<EmeraldGem, DexterityPower>
{
    public override LocString Title => OriginModel is GemModel gem ? gem.Title : base.Title;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        OriginModel is GemModel gem ? gem.HoverTips : base.ExtraHoverTips;
}