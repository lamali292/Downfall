using System.Collections.Generic;
using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;
using SlimeBoss.SlimeBossCode.Core;

namespace SlimeBoss.SlimeBossCode.Cards.Uncommon;

[Pool(typeof(SlimeBossCardPool))]
public class Repurpose : SlimeBossCardModel
{
    public Repurpose() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithKeyword(CardKeyword.Exhaust);
        WithCostUpgradeBy(-1);
        WithCards(2);
        WithPower<DrawCardsNextTurnPower>(1);
        WithTip<Slimed>();
    }

    protected override Artist Artist => Artist.Get<Thelethargicweirdo>();

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var prefs = new CardSelectorPrefs(CardSelectorPrefs.TransformSelectionPrompt, 0, DynamicVars.Cards.IntValue);
        var cards = await CardSelectCmd.FromCombatPile(ctx, PileType.Draw.GetPile(Owner), Owner, prefs);
        var transformations = new List<CardTransformation>();
        foreach (var card in cards)
        {
            var slimed = CombatState?.CreateCard<Slimed>(Owner);
            if (slimed == null) continue;
            transformations.Add(new CardTransformation(card, slimed));
        }
        if (transformations.Count == 0) return;

        await CardCmd.Transform(transformations, null);
        await CommonActions.ApplySelf<DrawCardsNextTurnPower>(ctx, this,
            transformations.Count * DynamicVars[nameof(DrawCardsNextTurnPower)].BaseValue);
    }
}
