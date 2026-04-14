using Downfall.Code.Abstract;
using Downfall.Code.Cards.CardModels;
using Downfall.Code.Interfaces;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.Code.Powers.Collector;

public class CopyNextTurnPower : CollectorPowerModel
{
    
    public CopyNextTurnPower() : base(PowerType.Buff, PowerStackType.Single)
    {
        WithVars(new CardDynamicVar());
        WithTip(new PowerTooltipSource(Tip));
    }

    private static IHoverTip Tip(PowerModel arg)
    {
        if (arg is CopyNextTurnPower { Card: not null } power)
        {
            return new CardHoverTip(power.Card);
        };
        return HoverTipFactory.Static(StaticHoverTip.None);
    }


    public override bool IsInstanced => true;
    public CardModel? Card;
    public Action<CardModel>? OnAdd;
    
    public override async Task BeforeHandDraw(Player player, PlayerChoiceContext choiceContext, CombatState combatState)
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