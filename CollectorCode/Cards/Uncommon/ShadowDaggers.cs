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
    public ShadowDaggers() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
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

    /// <summary>Torchhead hits what you pick; while it targets ALL enemies (Equip Axe) there is nothing to pick.</summary>
    public override TargetType TargetType => _owner != null && IsMutable && ShouldTorcheadTargetAll
        ? TargetType.AllEnemies
        : TargetType.AnyEnemy;

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var hits = (int)((CalculatedVar)DynamicVars["CalculatedHits"]).Calculate(cardPlay.Target);
        for (var i = 0; i < hits; i++)
        {
            // when torchead can get vigor, this needs to be changed
            await TorchheadCmd.Attack(ctx, this, cardPlay, attack => attack.WithHitFx("vfx/vfx_attack_slash"));
        }

    }
}