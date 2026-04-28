using BaseLib.Utils;
using Hexaghost.HexaghostCode.Core;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Hexaghost.HexaghostCode.Cards.Rare;

[Pool(typeof(HexaghostCardPool))]
public class TimeOfNeed : HexaghostCardModel
{
    public TimeOfNeed() : base(0, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        WithKeywords(CardKeyword.Exhaust, CardKeyword.Retain);
    }
    
    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var card = CardFactory.GetDistinctForCombat(Owner, Owner.Character.CardPool.GetUnlockedCards(Owner.UnlockState, Owner.RunState.CardMultiplayerConstraint).Where(c => c.Type == CardType.Power), 1, Owner.RunState.Rng.CombatCardGeneration).FirstOrDefault();
        if (card == null) return;
        card.SetToFreeThisTurn();
        if (IsUpgraded) card.UpgradeInternal();
        await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Hand, Owner);

    }


}