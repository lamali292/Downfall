using Downfall.Code.Abstract;
using Downfall.Code.Cards.Champ.Common;
using Downfall.Code.Events;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Downfall.Code.Powers.Champ;

public class CounterPower : ChampPowerModel
{
    public CounterPower()
    {
        WithTip(new PowerTooltipSource(GetPowerTooltip));
    }

    private static CardHoverTip GetPowerTooltip(PowerModel arg)
    {
        var card = ModelDb.Card<RiposteStrike>();
        card.DynamicVars.Damage.BaseValue = arg.Amount;
        return new CardHoverTip(card);
    }

    public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target,
        DamageResult damageResult, ValueProp props,
        Creature? dealer, CardModel? cardSource)
    {
        if (target != Owner || dealer == Owner || Owner.Player == null) return;
        var player = Owner.Player;
        var strikeCount = DownfallHook.ModifyCounterStrikeCount(player, 1);
        var cards = new List<CardModel>();
        for (var i = 0; i < strikeCount; i++)
        {
            var card = player.Creature.CombatState!.CreateCard(ModelDb.Card<RiposteStrike>(), player);
            card.DynamicVars.Damage.BaseValue = Amount;
            cards.Add(card);
        }

        await CardPileCmd.AddGeneratedCardsToCombat(cards, PileType.Hand, true);
        await PowerCmd.Remove(this);
    }
}