using BaseLib.Utils;
using Collector.CollectorCode.Cards.Token;
using Collector.CollectorCode.Core;
using Collector.CollectorCode.CustomEnums;
using Collector.CollectorCode.Extensions;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Encounters;
using MegaCrit.Sts2.Core.Models.Powers;
namespace Collector.CollectorCode.Cards.Collectibles;

public class KinPriestCard : Collectible<TheKinBoss>
{
    public KinPriestCard() : base(1, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy, 0.786f)
    {
        WithTorchheadDamage(8, 3);
        WithCalculatedVar("Hits", 1, Calc);
        WithTip(CollectorKeyword.Torchhead);
        WithTip<WeakPower>();
    }

    /// <summary>Torchhead attacks once, plus once more for each enemy with Weak.</summary>
    private static decimal Calc(CardModel card, Creature? creature) =>
        card.CombatState?.HittableEnemies.Count(enemy => enemy.HasPower<WeakPower>()) ?? 0;

    protected override bool ShouldGlowRedInternal => Owner.IsTorchheadMissing;

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var hits = (int)((CalculatedVar)DynamicVars["Hits"]).Calculate(null);
        for (var i = 0; i < hits; i++)
            await TorchheadCmd.AttackOrKindle(ctx, this, cardPlay);
    }
}