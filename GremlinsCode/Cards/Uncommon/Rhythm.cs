using BaseLib.Utils;
using Downfall.DownfallCode.Commands;
using Downfall.DownfallCode.CustomEnums;
using Gremlins.GremlinsCode.Core;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Gremlins.GremlinsCode.Cards.Uncommon;

[Pool(typeof(GremlinsCardPool))]
public class Rhythm : GremlinsCardModel
{
    public Rhythm() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithCostUpgradeBy(-1);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await GremlinsCmd.SwapToNext(ctx, Owner);
        var selected =
            (await DownfallCardCmd.SelectFromCombatPile(ctx, PileType.Draw.GetPile(Owner), DownfallCardSelectorPrefs.ToHandSelectionPrompt, this, e => e.Rarity == CardRarity.Basic))
            .FirstOrDefault();
        if (selected == null) return;
        selected.EnergyCost.SetThisTurn(0);
        await CardPileCmd.Add(selected, PileType.Hand);
    }
}