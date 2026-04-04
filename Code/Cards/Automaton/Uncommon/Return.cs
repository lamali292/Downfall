using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Cards.CardModels;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Downfall.Code.Cards.Automaton.Uncommon;

[Pool(typeof(AutomatonCardPool))]
public class Return : AutomatonCardModel
{
    public Return() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithVar("Energy", 1, 1);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var discardPile = PileType.Discard.GetPile(Owner);
        if (discardPile.Cards.Count == 0) return;

        var prefs = new CardSelectorPrefs(
            SelectionScreenPrompt,
            1, 1
        );

        var selected = await CardSelectCmd.FromSimpleGrid(
            ctx,
            discardPile.Cards.ToList(),
            Owner,
            prefs
        );

        foreach (var card in selected)
            await CardPileCmd.Add(card, PileType.Draw, CardPilePosition.Top);
        await PowerCmd.Apply<EnergyNextTurnPower>(Owner.Creature, DynamicVars.Energy.IntValue, Owner.Creature, this);
    }
}