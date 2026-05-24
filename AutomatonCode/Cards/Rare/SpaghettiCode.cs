using Automaton.AutomatonCode.Core;
using Automaton.AutomatonCode.CustomEnums;
using Automaton.AutomatonCode.Extensions;
using Automaton.AutomatonCode.Interfaces;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Automaton.AutomatonCode.Cards.Rare;

[Pool(typeof(AutomatonCardPool))]
public class SpaghettiCode : AutomatonCardModel
{
    public SpaghettiCode() : base(2, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        WithKeywords(CardKeyword.Exhaust);
        WithTip(AutomatonTip.Encode);
        WithCostUpgradeBy(-1);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var rng = Owner.RunState.Rng.CombatCardSelection;

        var cards = Owner.Character.CardPool
            .GetUnlockedCards(Owner.UnlockState, Owner.RunState.CardMultiplayerConstraint)
            .Where(c => AutomatonCmd.IsEncodable(c) && c.Rarity != CardRarity.Token).ToList();

        var max = AutomatonCmd.GetMax(Owner);
        while (Owner.GetEncode().Count < max)
        {
            var countBefore = Owner.GetEncode().Count;
            var choices = CardFactory.GetDistinctForCombat(Owner, cards, 3, rng).ToList();
            var selected = await CardSelectCmd.FromChooseACardScreen(ctx, choices, Owner);
            if (selected == null) break;
            await AutomatonCmd.EncodeCard(selected, ctx);
            if (Owner.GetEncode().Count < countBefore + 1)
                return;
        }
    }
}