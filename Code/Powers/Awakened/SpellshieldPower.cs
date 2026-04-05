using Downfall.Code.Abstract;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Downfall.Code.Powers.Awakened;

public class SpellshieldPower : AwakenedPowerModel
{
    public override async Task AfterCardRetained(CardModel card)
    {
        await CreatureCmd.GainBlock(card.Owner.Creature, Amount, ValueProp.Unpowered, null);
    }
}