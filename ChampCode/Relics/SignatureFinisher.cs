using BaseLib.Utils;
using Champ.ChampCode.Core;
using Champ.ChampCode.Enchantments;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models;

namespace Champ.ChampCode.Relics;
//todo add signature tip
[Pool(typeof(ChampRelicPool))]
public class SignatureFinisher() : ChampRelicModel(RelicRarity.Rare)
{
    public override bool HasUponPickupEffect => true;

    public override async Task AfterObtained()
    {
        var prefs = new CardSelectorPrefs(CardSelectorPrefs.EnchantSelectionPrompt, 1);
        var card = (await CardSelectCmd.FromDeckForEnchantment(Owner, ModelDb.Enchantment<Signature>(), 1, prefs))
            .FirstOrDefault();
        if (card == null) return;
        CardCmd.Enchant<Signature>(card, 1);
        CardCmd.Preview(card);
    }
}