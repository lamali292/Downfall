using Automaton.AutomatonCode.Core;
using Automaton.AutomatonCode.CustomEnums;
using Automaton.AutomatonCode.Enchantments;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models;

namespace Automaton.AutomatonCode.Relics;

[Pool(typeof(AutomatonRelicPool))]
public class BottledCode : AutomatonRelicModel
{
    public BottledCode() : base(RelicRarity.Rare)
    {
        WithTip<Hardcoded>();
        WithTip(AutomatonTip.Encode);
    }

    public override bool HasUponPickupEffect => true;


    public override async Task AfterObtained()
    {
        
        var prefs = new CardSelectorPrefs(CardSelectorPrefs.EnchantSelectionPrompt, 1);
        var card = (await CardSelectCmd.FromDeckForEnchantment(Owner, ModelDb.Enchantment<Hardcoded>(), 1, null, prefs))
            .FirstOrDefault();
        if (card == null) return;
        CardCmd.Enchant<Hardcoded>(card, 1);
        CardCmd.Preview(card);
    }
}