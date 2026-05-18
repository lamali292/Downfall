using Awakened.AwakenedCode.Core;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using Void = MegaCrit.Sts2.Core.Models.Cards.Void;

namespace Awakened.AwakenedCode.Cards.Common;

[Pool(typeof(AwakenedCardPool))]
public class Gloomguard : AwakenedCardModel
{
    public Gloomguard() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        WithBlock(8, 3);
        WithEnergyTip();
        WithTip(typeof(Void));
    }

    public override bool ShouldReceiveCombatHooks => true;
    protected override bool ShouldGlowGoldInternal => HasVoidInHand();


    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardBlock(this, cardPlay);
    }

    private bool HasVoidInHand()
    {
        return PileType.Hand.GetPile(Owner).Cards.Any(e => e.Id == ModelDb.Card<Void>().Id);
    }

    public override bool TryModifyEnergyCostInCombat(CardModel card, decimal originalCost, out decimal modifiedCost)
    {
        if (card == this && HasVoidInHand())
        {
            modifiedCost = 0;
            return true;
        }

        modifiedCost = originalCost;
        return false;
    }

    public override Task AfterCardChangedPiles(CardModel card, PileType oldPileType, AbstractModel? source)
    {
        if (card.Owner == Owner && (oldPileType == PileType.Hand || card.Pile?.Type == PileType.Hand))
            InvokeEnergyCostChanged();
        return Task.CompletedTask;
    }
}