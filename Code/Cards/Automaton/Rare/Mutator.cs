using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Cards.CardModels;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.Screens.CardSelection;
using MegaCrit.Sts2.Core.Nodes.Screens.Overlays;

namespace Downfall.Code.Cards.Automaton.Rare;

[Pool(typeof(AutomatonCardPool))]
public class Mutator() : AutomatonCardModel(1, CardType.Power, CardRarity.Rare, TargetType.None)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<StrengthPower>(2)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<StrengthPower>()
    ];

    private async Task<CardModel?> PickOne(List<CardModel> cards)
    {
        ArgumentNullException.ThrowIfNull(NOverlayStack.Instance);
        var screen = NSimpleCardSelectScreen.Create(cards, new CardSelectorPrefs(SelectionScreenPrompt, 1, 1));
        NOverlayStack.Instance.Push(screen);
        return (await screen.CardsSelected()).FirstOrDefault();
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await PowerCmd.Apply<StrengthPower>(Owner.Creature, DynamicVars["StrengthPower"].BaseValue, Owner.Creature,
            this);

        var statuses = PileType.Hand
            .GetPile(Owner)
            .Cards
            .Where(c => c.Type == CardType.Status)
            .ToList();
        if (statuses.Count == 0) return;

        var selected = statuses.Count == 1
            ? statuses.First()
            : await PickOne(statuses);

        if (selected == null) return;
        await CardCmd.Transform(selected, CreateClone());
    }

    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Retain);
    }
}