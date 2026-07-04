using Downfall.DownfallCode.Artists;
using Downfall.DownfallCode.Commands;
using Hermit.HermitCode.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace Hermit.HermitCode.Cards.Rare;

public sealed class FromBeyond : HermitCardModel
{
    public FromBeyond() : base(1, CardType.Skill, CardRarity.Rare, TargetType.RandomEnemy)
    {
        this.WithHpLoss(5, 2);
        WithCalculatedVar("CalculatedHits", 0, CountCardsInExhaust);
        WithTip(CardKeyword.Exhaust);
    }

    protected override Artist Artist => Artist.Get<AlexMdle>();

    private static decimal CountCardsInExhaust(CardModel card, Creature? _)
    {
        return card.Owner.GetExhaust().Count;
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        var exhaustCount = ((CalculatedVar)DynamicVars["CalculatedHits"]).Calculate(null);
        for (var i = 0; i < exhaustCount; i++)
        {
            var enemy = Owner.RunState.Rng.CombatTargets.NextItem(CombatState!.HittableEnemies);
            if (enemy == null) break;
            HermitCombatFx.GroundFireOnTarget(enemy);
            await MyCommonActions.LoseHpToTarget(ctx, this, enemy);
        }
    }
}