using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Cards.Awakened.Token;
using Downfall.Code.Cards.CardModels;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.CommonUi;

namespace Downfall.Code.Cards.Awakened.Rare;

[Pool(typeof(AwakenedCardPool))]
public class RealityRift : AwakenedCardModel
{
    public RealityRift() : base(0, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        WithKeywords(CardKeyword.Exhaust);
    }


    private static CardModel[] AnotherDimensionCards =>
    [
        ModelDb.Card<Crusher>(),
        ModelDb.Card<Daggerstorm>(),
        ModelDb.Card<ManaShield>(),
        ModelDb.Card<Minniegun>(),
        ModelDb.Card<Mantis>(),
        ModelDb.Card<Scheme>(),
        ModelDb.Card<SignInBlood>(),
        ModelDb.Card<SpreadingSpores>(),
        ModelDb.Card<TheEncyclopedia>()
    ];


    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var list = CardFactory
            .GetDistinctForCombat(Owner, AnotherDimensionCards, 3, Owner.RunState.Rng.CombatCardGeneration).ToList();
        if (IsUpgraded)
            CardCmd.Upgrade(list, CardPreviewStyle.HorizontalLayout);

        var card = await CardSelectCmd.FromChooseACardScreen(ctx, list, Owner, true);

        if (card == null)
            return;
        await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Hand, true);
    }
}