using BaseLib.Utils;
using Champ.ChampCode.Core;
using Champ.ChampCode.CustomEnums;
using Champ.ChampCode.Enchantments;
using Downfall.DownfallCode.Interfaces;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Entities.Players;

namespace Champ.ChampCode.Relics;

[Pool(typeof(ChampRelicPool))]
public class SignatureFinisher : ChampRelicModel, IRelicAllowedForPlayer
{
    public SignatureFinisher() : base(RelicRarity.Rare)
    {
        WithTip(ChampTip.Finisher);
        WithTip<Signature>();
    }

    public override bool HasUponPickupEffect => true;

    public bool IsAllowedForPlayer(Player player)
    {
        var signature = ModelDb.Enchantment<Signature>();
        return player.Deck.Cards.Any(signature.CanEnchant);
    }

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