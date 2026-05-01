using Awakened.AwakenedCode.Core;
using Awakened.AwakenedCode.CustomEnums;
using Awakened.AwakenedCode.Interfaces;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace Awakened.AwakenedCode.Cards.Token;

[Pool(typeof(TokenCardPool))]
public class ManaShield : AwakenedCardModel
{
    public ManaShield() : base(2, CardType.Skill, CardRarity.Token, TargetType.Self)
    {
        WithBlock(14, 4);
        WithTip(AwakenedTip.Conjure);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (CombatState == null) return;
        await CommonActions.CardBlock(this, cardPlay);
        await AwakenedCmd.Conjure(Owner, CombatState);
        var card = Owner.RunState.Rng.CombatCardGeneration
            .NextItem(PileType.Hand.GetPile(Owner).Cards.Where(c => c is ISpell && c.EnergyCost.GetResolved() > 0));
        card?.EnergyCost.UpgradeBy(-1);
    }
}