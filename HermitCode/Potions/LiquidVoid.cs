using Downfall.DownfallCode.CustomEnums;
using Hermit.HermitCode.Core;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Hermit.HermitCode.Potions;

public class LiquidVoid() : HermitPotionModel(PotionRarity.Rare, PotionUsage.CombatOnly, TargetType.Self)
{
    protected override async Task OnUse(PlayerChoiceContext ctx, Creature? target)
    {
        var prefs = new CardSelectorPrefs(DownfallCardSelectorPrefs.ToHandSelectionPrompt, 1);
        var card = (await CardSelectCmd.FromCombatPile(ctx, PileType.Exhaust.GetPile(Owner), Owner, prefs)).FirstOrDefault();
        if (card == null) return;
        card.SetToFreeThisTurn();
        await CardPileCmd.Add(card, PileType.Hand);
    }
}