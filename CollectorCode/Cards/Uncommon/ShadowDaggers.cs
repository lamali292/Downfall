using BaseLib.Utils;
using Collector.CollectorCode.Core;
using Collector.CollectorCode.Events;
using Collector.CollectorCode.Extensions;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace Collector.CollectorCode.Cards.Uncommon;

[Pool(typeof(CollectorCardPool))]
public class ShadowDaggers : CollectorCardModel
{
    public ShadowDaggers() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.Self)
    {
        WithTorchheadDamage(5, 3);
        WithCalculatedVar("CalculatedHits", 0, Calc);
        WithKeyword(CardKeyword.Exhaust);
    }

    protected override Artist Artist => Artist.Get<Opal>();
    
    protected override bool ShouldGlowRedInternal => Owner.IsTorchheadMissing;

    
    
    private static decimal Calc(CardModel card, Creature? creature)
    {
        return card.Owner.DeckPile.Count(c => c.VisualCardPool.IsColorless);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (Owner.IsTorchheadMissing) return;//If no Torchhead, do not run.
        var hits = (int)((CalculatedVar)DynamicVars["CalculatedHits"]).Calculate(cardPlay.Target);
        await (CollectorCmd.TorchheadAttack(this)?.WithHitCount(hits).WithHitFx("vfx/vfx_attack_slash")).ExecuteIfPresent(ctx);
    }
    
    protected override void AddExtraArgsToDescription(LocString description)
    {
        var shouldTargetAll = _owner != null && CollectorHook.ShouldTorchheadTargetAll(_owner, out _);
        description.Add("TorchheadTargetsAll", shouldTargetAll);
        base.AddExtraArgsToDescription(description);
    }
}