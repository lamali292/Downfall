using Godot;
using Guardian.GuardianCode.Cards;
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
using MegaCrit.Sts2.Core.Models.Powers.Mocks;

namespace Guardian.GuardianCode.Gems;

public class BismuthGem : GemModel
{
    public override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<ArtifactPower>(),
        HoverTipFactory.Static(GuardianTip.Aggravate)
    ];

    public override Color GemColor => new(0xD8786AFF);
    public override CardRarity Rarity => CardRarity.Rare;
    protected override IEnumerable<DynamicVar> CanonicalVars => [new GemVar(1)];

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay? cardPlay)
    {
        var effect = GuardianHook.ModifyGemEffect(CombatState, this, DynamicVars.Gem().BaseValue, Card);
        await PowerCmd.Apply<ArtifactPower>(ctx, Player.Creature, effect, Player.Creature, null);
    }
    
    /*
    public override void OnInitialApplication()
    {
        //if (Card is IGemCard or null) return;
        Card.EnergyCost.UpgradeBy(1);
    }*/

    public override bool TryModifyEnergyCostInCombat(CardModel card, decimal originalCost, out decimal modifiedCost)
    {
        modifiedCost = originalCost;
        if (Card is  IGemCard || card != Card) return false;
        modifiedCost++;;
        return true;
    }
}