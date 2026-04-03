using Downfall.Code.Abstract;
using Downfall.Code.Cards.Champ.Common;
using Downfall.Code.Commands;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Downfall.Code.Powers.Champ;

public class CounterPower : ChampPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<IHoverTip> ExtraHoverTips
    {
        get
        {
            var card = ModelDb.Card<RiposteStrike>();
            card.DynamicVars.Damage.BaseValue = Amount;
            return [new CardHoverTip(card)];
        }
    }

    public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult damageResult, ValueProp props,
        Creature? dealer, CardModel? cardSource)
    {
        if (target != Owner || dealer == Owner || Owner.Player == null) return;
        var player = Owner.Player;
        var card = player.Creature.CombatState!.CreateCard(ModelDb.Card<RiposteStrike>(), player);
        card.DynamicVars.Damage.BaseValue = Amount;
        var result = await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Hand, true);
        if (result.success)
            CardCmd.PreviewCardPileAdd(result, 0.2f);
        await PowerCmd.Remove(this);
    }
}