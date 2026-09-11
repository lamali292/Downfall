using BaseLib.Utils;
using Collector.CollectorCode.Core;
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
    public ShadowDaggers() : base(2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        WithTorchheadDamage(5, 3);
        WithCalculatedVar("CalculatedHits", 0, Calc);
        WithKeyword(CardKeyword.Exhaust);
    }

    protected override Artist Artist => Artist.Get<Opal>();
    
    protected override bool ShouldGlowRedInternal => Owner.IsTorchheadMissing;

    
    
    private static decimal Calc(CardModel card, Creature? creature)
    {
        return card.Owner.GetAllCombatCards.Count(c => c.VisualCardPool.IsColorless);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (Owner.IsTorchheadMissing) return;//If no Torchhead, do not run.
        
        var hits = (int)((CalculatedVar)DynamicVars["CalculatedHits"]).Calculate(cardPlay.Target);

        for (var v = 0; v < hits; v++)
        {
            if (Owner.IsTorchheadMissing) return;//Torchhead could die to thorns between hits.
            await CollectorCmd.TorchheadAttack(this)!.WithHitFx("vfx/vfx_attack_slash").ExecuteIfPresent(ctx);
        }
        
        await CommonActions.CardAttack(this, cardPlay, hits)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(ctx);
    }
}