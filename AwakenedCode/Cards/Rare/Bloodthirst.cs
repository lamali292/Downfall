using Awakened.AwakenedCode.Core;
using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Potions;

namespace Awakened.AwakenedCode.Cards.Rare;

[Pool(typeof(AwakenedCardPool))]
public class Bloodthirst : AwakenedCardModel
{
    public Bloodthirst() : base(2, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
    {
        WithDamage(20, 5);
        WithTip(CardKeyword.Exhaust);
        this.WithTip<PowerPotion>();
        WithTip(StaticHoverTip.Fatal);
    }

    protected override Artist Artist => Artist.Get<Opal>();


    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (cardPlay.Target == null) return;
        var shouldTriggerFatal = cardPlay.Target.Powers.All(p => p.ShouldOwnerDeathTriggerFatal());
        var attackCommand = await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
        if (!shouldTriggerFatal || !attackCommand.Results.Any(r => r.Any(g => g.WasTargetKilled)))
            return;
        var potion = ModelDb.Potion<PowerPotion>().ToMutable();
        await PotionCmd.TryToProcure(potion, Owner);
        await CardCmd.Exhaust(ctx, this);
    }
}