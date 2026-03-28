using BaseLib.Patches.Content;
using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Cards.CardModels;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using Void = MegaCrit.Sts2.Core.Models.Cards.Void;

namespace Downfall.Code.Cards.Awakened.Common;

[Pool(typeof(AwakenedCardPool))]
public class Gloomguard : AwakenedCardModel
{
    public Gloomguard() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        WithBlock(8);
    }

    public override bool ShouldReceiveCombatHooks => true;

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardBlock(this, cardPlay);
    }

    private bool HasVoidInHand()
        => PileType.Hand.GetPile(Owner).Cards.Any(e => e.Id == ModelDb.Card<Void>().Id);

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

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(3);
    }
}