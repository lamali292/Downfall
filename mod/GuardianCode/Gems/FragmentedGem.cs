using Downfall.DownfallCode.Commands;
using Godot;
using Guardian.GuardianCode.Cards.Token;
using Guardian.GuardianCode.Core;
using Guardian.GuardianCode.DynamicVars;
using Guardian.GuardianCode.Events;
using Guardian.GuardianCode.Extensions;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace Guardian.GuardianCode.Gems;

public class FragmentedGem : GemModel
{
    public override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromCard<CrystalShiv>()];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new GemVar(1)];
    public override Color GemColor => new(0xCE1AB2FF);
    public override CardRarity Rarity => CardRarity.Common;

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay? cardPlay,
        IEnumerable<Player> targetPlayers)
    {
        var effect = GuardianHook.ModifyGemEffect(CombatState, this, DynamicVars.Gem.BaseValue, Card);
        foreach (var player in targetPlayers)
            await DownfallCardCmd.GiveCards<CrystalShiv>(player, PileType.Hand, effect, creator: Player);
    }
}