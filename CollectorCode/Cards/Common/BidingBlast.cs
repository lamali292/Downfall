using BaseLib.Utils;
using Collector.CollectorCode.Core;
using Downfall.DownfallCode.CustomEnums;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Collector.CollectorCode.Cards.Uncommon;

[Pool(typeof(CollectorCardPool))]
public class BidingBlast : CollectorCardModel
{
    public BidingBlast() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
        WithDamage(9, 3);
        WithReserveTip();
        WithCards(1);
    }

    protected override bool ShouldGlowGoldInternal => CollectorEnergy.Instance?.Get(Owner) > 0 && Owner.PlayerCombatState?.Energy <= 0;
    
    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var reserve = CollectorEnergy.Instance;
        var usedReserve = reserve != null && reserve.WasSpentOn(this);
        var hits = 1;
        //if (usedReserve) hits++;
        await CommonActions.CardAttack(this, cardPlay, hits).Execute(ctx);
        if (usedReserve)
        {
            var prefs = new CardSelectorPrefs(DownfallCardSelectorPrefs.RetainSelectionPrompt, DynamicVars.Cards.IntValue);
            var cards = await CardSelectCmd.FromHand(ctx, Owner, prefs, c => !c.Keywords.Contains(CardKeyword.Retain),
                this);
            foreach (var cardModel in cards) CardCmd.ApplyKeyword(cardModel, CardKeyword.Retain);
        }
    }
}