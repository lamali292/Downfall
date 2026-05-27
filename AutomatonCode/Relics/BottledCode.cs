using Automaton.AutomatonCode.Core;
using Automaton.AutomatonCode.CustomEnums;
using Automaton.AutomatonCode.Enchantments;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models;

namespace Automaton.AutomatonCode.Relics;

[Pool(typeof(AutomatonRelicPool))]
public class BottledCode : AutomatonRelicModel
{
    public BottledCode() : base(RelicRarity.Rare)
    {
        WithTip(typeof(Encoding));
        WithTip(AutomatonTip.Encode);
        WithTip(CardKeyword.Exhaust);
    }

    public override bool HasUponPickupEffect => true;


    public override async Task AfterObtained()
    {
        var prefs = new CardSelectorPrefs(CardSelectorPrefs.EnchantSelectionPrompt, 1);
        var card = (await CardSelectCmd.FromDeckForEnchantment(Owner, ModelDb.Enchantment<Encoding>(), 1, e => e is
            {
                Type: CardType.Attack or CardType.Skill
            } && !e.Keywords.Contains(CardKeyword.Exhaust), prefs))
            .FirstOrDefault();
        if (card == null) return;
        CardCmd.Enchant<Encoding>(card, 1);
        CardCmd.Preview(card);
    }
}