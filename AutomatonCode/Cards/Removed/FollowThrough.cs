using Automaton.AutomatonCode.Core;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace Automaton.AutomatonCode.Cards.Removed;

[Obsolete]
[Pool(typeof(AutomatonCardPool))]
public class FollowThrough()
    : AutomatonCardModel(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy, false, false);