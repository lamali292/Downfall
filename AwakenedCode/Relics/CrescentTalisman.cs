using Awakened.AwakenedCode.Core;
using Awakened.AwakenedCode.CustomEnums;
using Awakened.AwakenedCode.Enchantments;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models;

namespace Awakened.AwakenedCode.Relics;

[Pool(typeof(AwakenedRelicPool))]
public class CrescentTalisman : AwakenedRelicModel
{
    public CrescentTalisman() : base(RelicRarity.Rare)
    {
        WithTip(typeof(Conjuration));
        WithTip(AwakenedTip.Conjure);
    }

    public override bool HasUponPickupEffect => true;


    public override async Task AfterObtained()
    {
        var prefs = new CardSelectorPrefs(CardSelectorPrefs.EnchantSelectionPrompt, 1);
        var card = (await CardSelectCmd.FromDeckForEnchantment(Owner, ModelDb.Enchantment<Conjuration>(), 1, prefs))
            .FirstOrDefault();
        if (card == null) return;
        CardCmd.Enchant<Conjuration>(card, 1);
        CardCmd.Preview(card);
    }
}