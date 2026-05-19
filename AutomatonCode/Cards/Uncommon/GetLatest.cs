using Automaton.AutomatonCode.Core;
using Automaton.AutomatonCode.CustomEnums;
using Automaton.AutomatonCode.Interfaces;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Automaton.AutomatonCode.Cards.Uncommon;

[Pool(typeof(AutomatonCardPool))]
public class GetLatest : AutomatonCardModel
{
    public GetLatest() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithKeywords(CardKeyword.Exhaust);
        WithTip(AutomatonTip.Encode);
        WithCostUpgradeBy(-1);
        WithEnergyTip();
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var encodableCards = ModelDb.AllCards
            .OfType<AutomatonCardModel>()
            .Where(c => c is IEncodable)
            .ToList();

        var random = Owner.RunState.Rng.CombatTargets.NextItem(encodableCards);
        if (random == null) return;

        var card = Owner.Creature.CombatState!.CreateCard(random, Owner);
        card.EnergyCost.SetThisTurnOrUntilPlayed(0);

        var result = await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Hand, Owner);
        if (result.success)
            CardCmd.PreviewCardPileAdd(result);
    }
}