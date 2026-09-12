using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Collector.CollectorCode.Cards.Collectibles.ActsFromThePast;

public class LagavulinClaw : ActsFromThePastCard
{
    public LagavulinClaw() : base(2, CardType.Skill, CardRarity.Uncommon, TargetType.AllEnemies, "LAGAVULIN_ELITE")
    {
        WithPower<DexterityPower>(5, 5);
        WithPower<LagavulinClawPower>(6, 2, false);
        WithTip<StrengthPower>();
        WithKeyword(CardKeyword.Exhaust);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await PowerCmd.Apply<DexterityPower>(ctx, CombatState!.HittableEnemies, -DynamicVars.Dexterity.BaseValue,
            Owner.Creature, this);
        await CommonActions.Apply<LagavulinClawPower>(ctx, this, cardPlay);
    }
}

public class LagavulinClawPower() : CustomTemporaryPowerModelWrapper<LagavulinClaw, StrengthPower>
{
    protected override bool InvertInternalPowerAmount => true;
}