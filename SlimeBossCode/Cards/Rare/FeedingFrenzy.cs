using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using SlimeBoss.SlimeBossCode.Core;
using SlimeBoss.SlimeBossCode.CustomEnums;
using BaseLib.Extensions;
using Downfall.DownfallCode.Artists;
using Downfall.DownfallCode.Compatibility;
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
        WithTip(SlimeBossTip.Consume);
        WithTip(CardKeyword.Exhaust);
    }

    public override bool CanBeGeneratedInCombat => false;

    protected override Artist Artist => Artist.Get<Thelethargicweirdo>();

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var weakTargets = SlimeBossCmd.WeakTargets(this, cardPlay);
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
        await SlimeBossCmd.Consume(ctx, this, weakTargets, async _ =>
        {
            await CreatureCmd.Heal(Owner.Creature, DynamicVars.Heal.BaseValue);
            await CardCmdCompatibility.Exhaust(ctx, this);
        });
    }
}
