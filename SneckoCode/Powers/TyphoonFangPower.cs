using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using Snecko.SneckoCode.Core;
using Snecko.SneckoCode.Events;

namespace Snecko.SneckoCode.Powers;

public class TyphoonFangPower : SneckoPowerModel, IAfterOverflowEffect
{
    private CardPlay? _pendingCardPlay;

    private bool _shouldTrigger;

    public TyphoonFangPower() : base(PowerType.Buff, PowerStackType.Single)
    {
        WithVars(new CardDynamicVar());
        WithTips(power =>
            power is TyphoonFangPower { Dupe: not null } fang ? [new CardHoverTip(fang.Dupe)] : []
        );
    }

    public override PowerInstanceType InstanceType => PowerInstanceType.Instanced;

    private CardModel? Dupe { get; set; }
    private CardModel? Source { get; set; }

    public async Task AfterOverflowEffect(PlayerChoiceContext ctx, CardPlay cardPlay, CardModel card)
    {
        if (card.Owner.Creature != Owner
            || Source == cardPlay.Card
            || Source == card
            || Dupe == null
            || cardPlay.IsAutoPlay) return;

        var enemy = CombatState.HittableEnemies
            .TakeRandom(1, CombatState.RunState.Rng.CombatTargets)
            .FirstOrDefault();

        var freshDupe = Source?.CreateDupe();
        Dupe = freshDupe;
        if (enemy == null || freshDupe == null || LocalContext.NetId == null) return;

        await CardCmd.AutoPlay(ctx, freshDupe, enemy);
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (!_shouldTrigger || _pendingCardPlay != cardPlay || Dupe == null) return;
        _shouldTrigger = false;
        _pendingCardPlay = null;
    }

    public void SetCard(CardModel card)
    {
        Dupe = card.CreateDupe();
        Source = card;
    }

    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side,
        IEnumerable<Creature> participants)
    {
        if (side != Owner.Side)
            return;
        await PowerCmd.Remove(this);
    }

    private class CardDynamicVar() : DynamicVar("card", 0)
    {
        public override string ToString()
        {
            return _owner is TyphoonFangPower power ? power.Dupe?.Title ?? "" : "";
        }
    }
}