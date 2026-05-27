using Collector.CollectorCode.Core;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace Collector.CollectorCode.Powers;

public class CopyNextTurnPower : CollectorPowerModel
{
    public CardModel? Card;
    public Action<CardModel>? OnAdd;

    public CopyNextTurnPower() : base(PowerType.Buff, PowerStackType.Single)
    {
        WithVars(new CardDynamicVar());
        WithTips(Tip);
    }

    public override PowerInstanceType InstanceType => PowerInstanceType.Instanced;

    private IEnumerable<IHoverTip> Tip(PowerModel arg)
    {
        return arg is CopyNextTurnPower { Card: not null } power ? [new CardHoverTip(power.Card)] : [];
    }


    public override async Task BeforeHandDraw(Player player, PlayerChoiceContext ctx, ICombatState combatState)
    {
        if (player.Creature != Owner || Card == null) return;
        await CardPileCmd.Add(Card, PileType.Hand);
        OnAdd?.Invoke(Card);
        await PowerCmd.Remove(this);
    }


    private class CardDynamicVar() : DynamicVar("card", 0)
    {
        private CopyNextTurnPower? _power;

        public override void SetOwner(AbstractModel model)
        {
            base.SetOwner(model);
            _power = model as CopyNextTurnPower;
        }

        public override string ToString()
        {
            return _power?.Card == null ? "?" : _power.Card.Title;
        }
    }
}