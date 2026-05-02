using Godot;
using Guardian.GuardianCode.Core;
using Guardian.GuardianCode.DynamicVars;
using Guardian.GuardianCode.Events;
using Guardian.GuardianCode.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Guardian.GuardianCode.Gems;

public class RutileGem : GemModel
{
    public override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<WeakPower>()];
    public override Color GemColor => new(0xDDCA8FFF);
    protected override IEnumerable<DynamicVar> CanonicalVars => [new GemVar(1)];
    public override CardRarity Rarity => CardRarity.Uncommon;

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay? cardPlay)
    {
        var effect = GuardianHook.ModifyGemEffect(CombatState, this, DynamicVars.Gem().BaseValue, Card);
        await PowerCmd.Apply<WeakPower>(ctx, CombatState.HittableEnemies, effect, Player.Creature,
            Card);
    }
}