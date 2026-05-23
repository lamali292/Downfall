using Automaton.AutomatonCode.Core;
using Automaton.AutomatonCode.CustomEnums;
using Automaton.AutomatonCode.Extensions;
using Automaton.AutomatonCode.Interfaces;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Extensions;
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
        var rng = CombatState!.RunState.Rng.CombatCardSelection;

        while (Owner.GetEncode().Count < AutomatonCmd.GetMax(Owner))
        {
            var countBefore = Owner.GetEncode().Count;
            var choices = Pool
                .AllCards
                .Where(c => c is IEncodable { AutoEncode: true } && c.Rarity != CardRarity.Token)
                .TakeRandom(3, rng)
                .Select(t => CombatState!.CreateCard(t, Owner))
                .ToList();

            var selected = await CardSelectCmd.FromChooseACardScreen(ctx, choices, Owner);
            foreach (var c in choices.Where(c => c != selected))
                c.RemoveFromState();
            if (selected == null) break;
            await AutomatonCmd.EncodeCard(selected, ctx, cardPlay);
            if (Owner.GetEncode().Count < countBefore + 1)
                return;
        }
    }
}