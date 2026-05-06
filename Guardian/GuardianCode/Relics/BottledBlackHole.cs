using BaseLib.Utils;
using Guardian.GuardianCode.Core;
using Guardian.GuardianCode.Enchantments;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models;

namespace Guardian.GuardianCode.Relics;

[Pool(typeof(GuardianRelicPool))]
public class BottledBlackHole() : GuardianRelicModel(RelicRarity.Uncommon)
{
      
    public override bool HasUponPickupEffect => true;
    public override async Task AfterObtained()
    {
        var prefs = new CardSelectorPrefs(CardSelectorPrefs.EnchantSelectionPrompt, 1);
        var card = (await CardSelectCmd.FromDeckForEnchantment(Owner, ModelDb.Enchantment<Temporal>(), 1, prefs))
            .FirstOrDefault();
        if (card == null) return;
        CardCmd.Enchant<Temporal>(card, 1);
        CardCmd.Preview(card);
    }
}