using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using Collector.CollectorCode.CustomEnums;
using Collector.CollectorCode.DynamicVars;
using Collector.CollectorCode.Events;
using Collector.CollectorCode.Core;
using Downfall.DownfallCode.Abstract;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Collector.CollectorCode.Cards;

public abstract class CollectorCardModel
    : DownfallCardModel<Core.Collector>
{
    protected CollectorCardModel(
        int cost,
        CardType type,
        CardRarity rarity,
        TargetType targetType,
        bool showInCardLibrary = true,
        bool autoAdd = true) : base(cost, type, rarity, targetType, showInCardLibrary, autoAdd)
    {
        WithTips(e => e.Keywords.Contains(CollectorKeyword.Pyre)
            ?
            [
                HoverTipFactory.FromKeyword(CardKeyword.Exhaust)
            ]
            : []);
    }
    
    
    protected bool ShouldTorcheadTargetAll => _owner != null && CollectorHook.ShouldTorchheadTargetAll(_owner, out _);
    
    /// <summary>
    /// Every Collector card exposes "TorchheadTargetsAll" to its description so Torchhead-attack cards can
    /// switch wording between "lowest-HP enemy" and "ALL enemies". Cards that never reference the arg are unaffected.
    /// </summary>
    protected override void AddExtraArgsToDescription(LocString description)
    {
        description.Add("TorchheadTargetsAll", ShouldTorcheadTargetAll);
        base.AddExtraArgsToDescription(description);
    }

    private bool IsIgnite => Keywords.Contains(CollectorKeyword.Ignite);

    /// <summary>
    ///     Set when this Ignite card moved from the Play pile straight into Exhaust, i.e. its own play exhausted it
    ///     (it has the Exhaust keyword). That exhaust must not count as "something else exhausted me", or the effect
    ///     would trigger twice. Uses the pile-change hook rather than the result-pile method so it works on every
    ///     supported game version.
    /// </summary>
    private bool _exhaustedByOwnPlay;

    /// <summary>
    ///     The effect of an Ignite card. It runs when the card is played and when anything else Exhausts it (Pyre,
    ///     Ethereal, ...), like Sly does for discards. Ignite cards implement their effect here instead of in
    ///     <see cref="OnPlayInternal">OnPlayInternal</see>/<c>AfterCardExhausted</c>.
    /// </summary>
    protected virtual Task OnIgnite(PlayerChoiceContext ctx) => Task.CompletedTask;

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (IsIgnite) await OnIgnite(ctx);
    }

    public override Task AfterCardChangedPiles(CardModel card, PileType oldPileType, AbstractModel? clonedBy)
    {
        if (card == this && IsIgnite) _exhaustedByOwnPlay = oldPileType == PileType.Play && Pile?.Type == PileType.Exhaust;
        return base.AfterCardChangedPiles(card, oldPileType, clonedBy);
    }

    public override async Task AfterCardExhausted(PlayerChoiceContext ctx, CardModel card, bool causedByEthereal)
    {
        if (card != this || !IsIgnite || CombatState == null) return;
        if (_exhaustedByOwnPlay)
        {
            _exhaustedByOwnPlay = false;
            return;
        }

        var playCount = await GeneratePlayCount(CombatState, null);
        for (var i = 0; i < playCount; ++i) await OnIgnite(ctx);
    }

    protected override bool IsPlayable => !IsBlockedByMissingPyreTarget;

    /// <summary>True when this card has Pyre/Megapyre and there's no other hand card to exhaust for it.</summary>
    private bool IsBlockedByMissingPyreTarget => HasPyre && Owner.Hand.All(e => e == this);

    private bool HasPyre => Keywords.Contains(CollectorKeyword.Pyre) || Keywords.Contains(CollectorKeyword.Megapyre);
    
    protected ConstructedCardModel WithKindle(int baseVal, int upgradeVal = 0)
    {
        return WithVar(new KindleVar(baseVal).WithUpgrade(upgradeVal));
    }
    
    protected ConstructedCardModel WithReserve(int baseVal, int upgradeVal = 0)
    {
        WithReserveTip();
        return WithVar(new ReserveVar(baseVal).WithUpgrade(upgradeVal));
    }
    
    protected ConstructedCardModel WithTorchheadDamage(int baseVal, int upgradeVal = 0)
    {
        return WithVar(new TorchheadDamageVar(baseVal, DamageProps.card).WithUpgrade(upgradeVal));
    }



    protected ConstructedCardModel WithReserveTip()
    {
        return WithTip(new TooltipSource(_ => CollectorTip.ReserveTip));
    }

}