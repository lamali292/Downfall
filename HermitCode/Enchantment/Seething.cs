using Downfall.DownfallCode.Abstract;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Enchantments;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace Hermit.HermitCode.Enchantment;

public class Seething : DownfallEnchantmentModel<Core.Hermit>
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(1),
        new EnergyVar(1)
    ];

    public override bool CanEnchant(CardModel card) => true;

    public override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay? cardPlay)
    {
        if (Status != EnchantmentStatus.Normal)
            return;
        Status = EnchantmentStatus.Disabled;
        await CardPileCmd.Add(Card, PileType.Hand);
        await PlayerCmd.GainEnergy(DynamicVars.Energy.IntValue, Card.Owner);
        await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.IntValue, Card.Owner);
    }
    
    public override bool HasExtraCardText => Status == EnchantmentStatus.Normal;

    public override Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side, IReadOnlyList<Creature> participants,
        ICombatState combatState)
    {
        if (!participants.Contains(Card.Owner.Creature))  return Task.CompletedTask;
        Status = EnchantmentStatus.Normal;
        return Task.CompletedTask;
    }
}