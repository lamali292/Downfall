using BaseLib.Utils;
using Collector.CollectorCode.Core;
using Collector.CollectorCode.CustomEnums;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Enchantments;
namespace Collector.CollectorCode.Cards.Rare;

[Pool(typeof(CollectorCardPool))]
public class ReceiveTribute : CollectorCardModel
{
    public ReceiveTribute() : base(0, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        WithKeyword(CollectorKeyword.Pyre);
        WithKeyword(CardKeyword.Exhaust);
        WithCards(3);
        WithEnchantment<Steady>();
    }

    protected override Artist Artist => Artist.Get<Opal>();

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var list = ModelDb
            .CardPool<CollectibleCardPool>()
            .AllCards
            .TakeRandom(DynamicVars.Cards.IntValue, Owner.RunState.Rng.CombatCardGeneration)
            .Select(Select)
            .ToList();

        var card = await CardSelectCmd.FromChooseACardScreen(ctx, list, Owner, true);
        if (card == null)
            return;
        CardCmd.Enchant<Steady>(card, DynamicVars.Enchantment<Steady>().IntValue);
        await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Hand, Owner);
    }
    
    private CardModel Select(CardModel cardModel)
    {
        if (RunState == null) throw new InvalidOperationException();
        var card = CombatState!.CreateCard(cardModel, Owner);
        if (IsUpgraded)
            CardCmd.Upgrade(card);
        return card;
    }
}