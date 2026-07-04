using Champ.ChampCode.CustomEnums;
using Downfall.DownfallCode.Abstract;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;

namespace Champ.ChampCode.Enchantments;

public class Signature : DownfallEnchantmentModel<Core.Champ>
{
    public override bool CanEnchant(CardModel card)
    {
        return base.CanEnchant(card) && card.Tags.Contains(ChampTag.Finisher);
    }

    protected override void OnEnchant()
    {
        Card.EnergyCost.UpgradeBy(-Card.EnergyCost.GetWithModifiers(CostModifiers.None));
    }
}