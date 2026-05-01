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
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Guardian.GuardianCode.Gems;

public class BismuthGem : GemModel
{
    public override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<ArtifactPower>()];
    public override Color GemColor => new(0xD8786AFF);
    public override CardRarity Rarity => CardRarity.Rare;
    protected override IEnumerable<DynamicVar> CanonicalVars => [new GemVar(1)];

    public override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var effect = GuardianHook.ModifyGemEffect(CombatState, this, DynamicVars.Gem().BaseValue, Card);
        await PowerCmd.Apply<ArtifactPower>(ctx, Owner.Creature, effect, Owner.Creature, null);
    }

    protected override void OnAdded(CardModel card)
    {
        card.EnergyCost.UpgradeBy(1);
    }

    protected override void OnRemoved(CardModel card)
    {
        card.EnergyCost.UpgradeBy(-1);
    }
}