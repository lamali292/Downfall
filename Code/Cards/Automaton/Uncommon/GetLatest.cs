using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Cards.CardModels;
using Downfall.Code.Interfaces;
using Downfall.Code.Keywords;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.Code.Cards.Automaton.Uncommon;

[Pool(typeof(AutomatonCardPool))]
public class GetLatest() : AutomatonCardModel(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    public override HashSet<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(DownfallKeywords.Encode)
    ];

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

        var result = await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Hand, true);
        if (result.success)
            CardCmd.PreviewCardPileAdd(result);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}