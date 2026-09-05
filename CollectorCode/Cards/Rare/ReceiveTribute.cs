using BaseLib.Utils;
using Collector.CollectorCode.Core;
using Collector.CollectorCode.CustomEnums;
using Collector.CollectorCode.Interfaces;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Enchantments;
using MegaCrit.Sts2.Core.Nodes.CommonUi;

namespace Collector.CollectorCode.Cards.Rare;

[Pool(typeof(CollectorCardPool))]
public class ReceiveTribute : CollectorCardModel
{
    // Todo: nah
    public ReceiveTribute() : base(0, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        WithKeyword(CollectorKeyword.Pyre);
        WithKeyword(CardKeyword.Exhaust);
        WithCards(2, 2);
        WithEnchantment<Steady>();
    }

    protected override Artist Artist => Artist.Get<Opal>();

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var list = CardFactory.GetDistinctForCombat(Owner, 
            ModelDb.CardPool<ColorlessCardPool>()
                .GetUnlockedCards(Owner.UnlockState, Owner.RunState.CardMultiplayerConstraint), 
            DynamicVars.Cards.IntValue, 
            Owner.RunState.Rng.CombatCardGeneration)
            .ToList();
        var card = await CardSelectCmd.FromChooseACardScreen(ctx, list, Owner, true);
        if (card == null)
            return;
        CardCmd.Enchant<Steady>(card, DynamicVars.Enchantment<Steady>().IntValue);
        await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Hand, Owner);
    }
}