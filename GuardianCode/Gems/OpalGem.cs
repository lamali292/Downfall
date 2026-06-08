using Godot;
using Guardian.GuardianCode.Core;
using Guardian.GuardianCode.DynamicVars;
using Guardian.GuardianCode.Events;
using Guardian.GuardianCode.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace Guardian.GuardianCode.Gems;

public class OpalGem : GemModel
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new GemVar(1)];
    public override Color GemColor => new(0xC7C7C7FF);
    public override CardRarity Rarity => CardRarity.Uncommon;

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay? cardPlay)
    {
        var effect = GuardianHook.ModifyGemEffect(CombatState, this, DynamicVars.Gem().BaseValue, Card);
        await CardPileCmd.Draw(ctx, effect, Player);
    }
}