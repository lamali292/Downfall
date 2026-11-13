using Champ.ChampCode.CustomEnums;
using Champ.ChampCode.Events;
using Downfall.DownfallCode.Abstract;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;

namespace Champ.ChampCode.Enchantments;

public class Signature : DownfallEnchantmentModel<Core.Champ>, IAllowFinisherWithoutStance, IKeepStanceAfterFinisher
{
    public bool AllowFinisherWithoutStance(CardModel card) => card == Card;

    public bool KeepStanceAfterFinisher(CardModel card) => card == Card;

    public override bool CanEnchant(CardModel card)
    {
        return base.CanEnchant(card) && card.Tags.Contains(ChampTag.Finisher);
    }

    protected override void OnEnchant()
    {
        Card.EnergyCost.UpgradeBy(-Card.EnergyCost.GetWithModifiers(CostModifiers.None));
        Card.EnergyCost.FinalizeUpgrade();
    }
}