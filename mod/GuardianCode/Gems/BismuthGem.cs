using Godot;
using Guardian.GuardianCode.Cards.Abstract;
using Guardian.GuardianCode.Core;
using Guardian.GuardianCode.CustomEnums;
using Guardian.GuardianCode.DynamicVars;
using Guardian.GuardianCode.Events;
using Guardian.GuardianCode.Extensions;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace Guardian.GuardianCode.Gems;

public class BismuthGem : GemModel
{
    public override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.Static(GuardianTip.Aggravate),
        HoverTipFactory.Static(GuardianTip.Stasis)
    ];

    public override Color GemColor => new(0xD8786AFF);
    public override CardRarity Rarity => CardRarity.Rare;
    protected override IEnumerable<DynamicVar> CanonicalVars => [new GemVar(1)];

    protected override Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay? cardPlay,
        IEnumerable<Player> targetPlayers)
    {
        var effect = GuardianHook.ModifyGemEffect(CombatState, this, DynamicVars.Gem.BaseValue, Card);
        foreach (var player in targetPlayers) GuardianCmd.AddMaxStasisSlots(player, (int)effect);

        return Task.CompletedTask;
    }


    public override bool TryModifyEnergyCostInCombat(CardModel card, decimal originalCost, out decimal modifiedCost)
    {
        modifiedCost = originalCost;
        if (Card is IGemCard || card != Card) return false;
        modifiedCost++;
        return true;
    }
}