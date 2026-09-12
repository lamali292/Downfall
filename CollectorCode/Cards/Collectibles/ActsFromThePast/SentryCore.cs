using BaseLib.Utils;
using Downfall.DownfallCode.Commands;
using Downfall.DownfallCode.Compatibility;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace Collector.CollectorCode.Cards.Collectibles.ActsFromThePast;

public class SentryCore : ActsFromThePastCard
{
    public SentryCore() : base(0, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy, "SENTRIES_ELITE")
    {
        WithDamage(6, 3);
        WithPower<WeakPower>(1);
        WithTip<Dazed>();
        WithCalculatedVar("Repeat", 0, Calc);
    }

    private static decimal Calc(CardModel card, Creature? _)
    {
        return 1+card.Owner.ExhaustPile.Count(e => e is Dazed);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (cardPlay.Target == null || CombatState == null) return;
        var repeat = ((CalculatedVar)DynamicVars["Repeat"]).Calculate(cardPlay.Target);
        var context = await AttackCommand.CreateContextAsync(CombatState, ctx, cardPlay);
        for (var i = 0; i < repeat; i++)
        {
            context.AddHit(await CompatibilityCreatureCmd.Damage(ctx,cardPlay.Target, DynamicVars.Damage.BaseValue, DynamicVars.Damage.Props, Owner.Creature, this, cardPlay));
            await CommonActions.Apply<WeakPower>(ctx, this, cardPlay);
            await DownfallCardCmd.GiveCard<Dazed>(Owner, PileType.Draw, CardPilePosition.Top);
        }
        await context.DisposeAsync();
    }
}