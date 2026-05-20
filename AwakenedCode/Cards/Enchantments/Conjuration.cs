using Awakened.AwakenedCode.Core;
using Awakened.AwakenedCode.CustomEnums;
using Downfall.DownfallCode.Abstract;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;

namespace Awakened.AwakenedCode.Cards.Enchantments;

public class Conjuration : DownfallEnchantmentModel<Core.Awakened>
{
    public override bool CanEnchant(CardModel card) => !card.Tags.Contains(AwakenedTag.Conjure);

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.Static(AwakenedTip.Conjure)
    ];

    public override Task OnPlay(PlayerChoiceContext choiceContext, CardPlay? cardPlay)
     => cardPlay?.Card.CombatState != null ?
         AwakenedCmd.Conjure(cardPlay.Card.Owner, cardPlay.Card.CombatState) :
         Task.CompletedTask;
    

    public override bool HasExtraCardText => true;

}