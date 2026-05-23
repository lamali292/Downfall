using Godot;
using Guardian.GuardianCode.Cards.Abstract;
using Guardian.GuardianCode.Core;
using Guardian.GuardianCode.CustomEnums;
using Guardian.GuardianCode.DynamicVars;
using Guardian.GuardianCode.Events;
using Guardian.GuardianCode.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Guardian.GuardianCode.Gems;

public class BismuthGem : GemModel
{
    public override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<ArtifactPower>(),
        HoverTipFactory.Static(StaticHoverTip.Energy),
        HoverTipFactory.Static(GuardianTip.Aggravate)
    ];

    public override Color GemColor => new(0xD8786AFF);
    public override CardRarity Rarity => CardRarity.Rare;
    protected override IEnumerable<DynamicVar> CanonicalVars => [new GemVar(1)];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay? cardPlay)
    {
        var effect = GuardianHook.ModifyGemEffect(CombatState, this, DynamicVars.Gem().BaseValue, Card);
        await PowerCmd.Apply<ArtifactPower>(ctx, Player.Creature, effect, Player.Creature, null);
    }

    protected override void OnAdded(CardModel card)
    {
        if (card is IGemCard) return;
        if (!card.IsInCombat) return;
        card.EnergyCost.UpgradeBy(1);
        card.EnergyCost.FinalizeUpgrade();
    }
}