using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Runs.History;

namespace Collector.CollectorCode.Extensions;

public static class JustFuckingEnchant
{
    /// <summary>
    /// Be very careful using this method, it does not provide ANY checks for whether the card should be enchanted so will crash if the card already has an enchantment.
    /// You should almost always use CardCmd.Enchant instead of this method.
    /// </summary>
    public static T? Enchant<T>(CardModel card, Decimal amount) where T : EnchantmentModel
    {
        return EnchantButGood(ModelDb.Enchantment<T>().ToMutable(), card, amount) as T;
    }
    
    /// <summary>
    /// Forcefully applies an enchantment to a card without checking if it is supposed to.
    /// </summary>
    /// <param name="enchantment"> An enchantment you must check is valid BEFORE calling this method.</param>
    /// <param name="card"> A card that normally wouldn't be allowed to get an enchantment (I.E Curses).</param>
    /// <param name="amount"> The amount of stacks of this enchantment.</param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static EnchantmentModel? EnchantButGood(
        EnchantmentModel enchantment,
        CardModel card,
        Decimal amount)
    {
        enchantment.AssertMutable();
        if (card.Enchantment == null)
        {
            card.EnchantInternal(enchantment, amount);
            enchantment.ModifyCard();
        }
        else if (card.Enchantment.GetType() == enchantment.GetType())
        {
            card.Enchantment.Amount += (int)amount;
        }
        card.FinalizeUpgradeInternal();
        CardPile pile = card.Pile;
        if (pile != null && pile.Type == PileType.Deck)
            card.Owner.RunState.CurrentMapPointHistoryEntry?.GetEntry(card.Owner.NetId).CardsEnchanted.Add(new CardEnchantmentHistoryEntry(card, enchantment.Id));
        return card.Enchantment;
    }
}