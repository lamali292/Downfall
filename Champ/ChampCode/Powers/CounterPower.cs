using Champ.ChampCode.Cards.Common;
using Champ.ChampCode.Core;
using Champ.ChampCode.Events;
using Downfall.DownfallCode.Abstract;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Champ.ChampCode.Powers;

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

    public override async Task AfterDamageReceived(PlayerChoiceContext ctx, Creature target,
        DamageResult damageResult, ValueProp props,
        Creature? dealer, CardModel? cardSource)
    {
        if (target != Owner || dealer == Owner || Owner.Player == null) return;
        var player = Owner.Player;
        var strikeCount = ChampHook.ModifyCounterStrikeCount(CombatState, player, 1);
        var cards = new List<CardModel>();
        for (var i = 0; i < strikeCount; i++)
        {
            var card = player.Creature.CombatState!.CreateCard(ModelDb.Card<RiposteStrike>(), player);
            card.DynamicVars.Damage.BaseValue = Amount;
            cards.Add(card);
        }

        await CardPileCmd.AddGeneratedCardsToCombat(cards, PileType.Hand, Owner.Player);
        await PowerCmd.ModifyAmount(ctx, this, -Amount, Owner, null);
    }
}