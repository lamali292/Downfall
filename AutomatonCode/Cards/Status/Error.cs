using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace Automaton.AutomatonCode.Cards.Status;

[Pool(typeof(StatusCardPool))]
public class Error : AutomatonCardModel
{
    public override int MaxUpgradeLevel => 0;
    public Error() : base(1, CardType.Status, CardRarity.Status, TargetType.Self)
    {
        WithKeyword(CardKeyword.Exhaust);
    }
}