using BaseLib.Utils;
using Downfall.DownfallCode.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Enchantments;
using Downfall.DownfallCode.DynamicVars;
using MegaCrit.Sts2.Core.Commands;
using SlimeBoss.SlimeBossCode.Core;
using SlimeBoss.SlimeBossCode.CustomEnums;

namespace SlimeBoss.SlimeBossCode.Relics;

[Pool(typeof(SlimeBossRelicPool))]
public class ProtectiveGear : SlimeBossRelicModel
{
    public ProtectiveGear() : base(RelicRarity.Shop)
    {
        WithVars(new EnchantmentVar<Adroit>(3));
        WithTips(_ => HoverTipFactory.FromEnchantment<Adroit>(3));
    }

    public override bool TryModifyCardBeingAddedToDeck(CardModel card, out CardModel? newCard)
    {
        newCard = null;
        if (card.Owner != Owner || 
            !card.Tags.Contains(SlimeBossTag.Tackle) || 
            !ModelDb.Enchantment<Adroit>().CanEnchant(card)) return false;
      
        DownfallCardCmd.Enchant<Adroit>(card, DynamicVars.Enchantment<Adroit>().BaseValue);
        newCard = card;
        return true;
    }
}