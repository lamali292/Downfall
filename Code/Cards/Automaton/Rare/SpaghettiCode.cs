using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Cards.CardModels;
using Downfall.Code.Commands;
using Downfall.Code.Interfaces;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Downfall.Code.Cards.Automaton.Rare;

[Pool(typeof(AutomatonCardPool))]
public class SpaghettiCode : AutomatonCardModel
{
    public SpaghettiCode() : base(2, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        WithKeywords(CardKeyword.Exhaust);
    }
 
    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var rng = CombatState!.RunState.Rng.CombatCardSelection;

        while (AutomatonCmd.GetSequenceCount(Owner) < AutomatonCmd.GetMax(Owner))
        {
            var countBefore = AutomatonCmd.GetSequenceCount(Owner);
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
            if (AutomatonCmd.GetSequenceCount(Owner) < countBefore + 1)
                return;
        }
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}