using BaseLib.Utils;
using Collector.CollectorCode.Core;
using Collector.CollectorCode.CustomEnums;
using Collector.CollectorCode.Extensions;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace Collector.CollectorCode.Cards.Uncommon;

[Pool(typeof(CollectorCardPool))]
public class ShadowDaggers : CollectorCardModel
{
    public ShadowDaggers() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.RandomEnemy)
    {
        WithTorchheadDamage(5, 3);
        WithCalculatedVar("CalculatedHits", 0, Calc);
        WithKeyword(CardKeyword.Exhaust);
        WithTip(CollectorKeyword.Torchhead);
    }

    protected override Artist Artist => Artist.Get<Opal>();
    

    
    
    private static decimal Calc(CardModel card, Creature? creature)
    {
        return card.Owner.DeckPile.Count(c => c.VisualCardPool.IsColorless);
    }

    /// <summary>Every hit picks a random enemy; with Equip Axe every hit lands on ALL enemies instead.</summary>
    public override TargetType TargetType => _owner != null && IsMutable && ShouldTorcheadTargetAll
        ? TargetType.AllEnemies
        : TargetType.RandomEnemy;

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var hits = (int)((CalculatedVar)DynamicVars["CalculatedHits"]).Calculate(cardPlay.Target);
        if (hits <= 0) return;
        // one attack with a hit count: Equip: Staff / Shield trigger once, not once per hit
        await TorchheadCmd.TorchheadAttack(this, cardPlay, hits, true).ExecuteIfPresent(ctx);
    }
}