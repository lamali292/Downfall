using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Cards.CardModels;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Potions;

namespace Downfall.Code.Cards.Awakened.Rare;

[Pool(typeof(AwakenedCardPool))]
public class Bloodthirst : AwakenedCardModel
{
    public Bloodthirst() : base(2, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
    {
        WithDamage(20);
        WithTip(CardKeyword.Exhaust);
        WithTip(typeof(PowerPotion));
        WithTip(new TooltipSource(_ => HoverTipFactory.Static(StaticHoverTip.Fatal)));
    }


    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (cardPlay.Target == null) return;
        var shouldTriggerFatal = cardPlay.Target.Powers.All(p => p.ShouldOwnerDeathTriggerFatal());
        var attackCommand = await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
        if (!shouldTriggerFatal || !attackCommand.Results.Any(r => r.WasTargetKilled))
            return;
        var potion = ModelDb.Potion<PowerPotion>().ToMutable();
        await PotionCmd.TryToProcure(potion, Owner);
        await CardCmd.Exhaust(ctx, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(5);
    }
}