using Automaton.AutomatonCode.Core;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace Automaton.AutomatonCode.Cards.Status;

[Pool(typeof(AutomatonCardPool))]
public class Error : AutomatonCardModel
{
    public Error() : base(1, CardType.Status, CardRarity.Status, TargetType.Self)
    {
        WithKeyword(CardKeyword.Exhaust);
    }
}