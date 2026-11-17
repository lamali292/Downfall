using MegaCrit.Sts2.Core.Models.Powers;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using SlimeBoss.SlimeBossCode.Core;
using SlimeBoss.SlimeBossCode.CustomEnums;
using BaseLib.Extensions;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace SlimeBoss.SlimeBossCode.Cards.Rare;

[Pool(typeof(SlimeBossCardPool))]
public class FeedingFrenzy : SlimeBossCardModel
{
    public FeedingFrenzy() : base(1, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
    {
        WithDamage(6, 2);
        WithHeal(6, 2);
        WithTip<WeakPower>();
        WithTip(CardKeyword.Exhaust);
    }

    public override bool CanBeGeneratedInCombat => false;

    protected override Artist Artist => Artist.Get<Thelethargicweirdo>();

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        // Checked before the attack: an enemy it kills has already lost its Weak, but still counts.
        var hadWeak = cardPlay.Target?.GetPowerAmount<WeakPower>() > 0;
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
        if (!hadWeak) return;
        if (cardPlay.Target?.GetPower<WeakPower>() is { Amount: > 0 } weak)
            await PowerCmd.ModifyAmount(ctx, weak, -1, Owner.Creature, this);
        await CreatureCmd.Heal(Owner.Creature, DynamicVars.Heal.BaseValue);
        await CardCmd.Exhaust(ctx, this);
    }
}
