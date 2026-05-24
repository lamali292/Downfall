using Automaton.AutomatonCode.Core;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace Automaton.AutomatonCode.Cards.Removed;

[Obsolete]
[Pool(typeof(AutomatonCardPool))]
public class DelayedGuard() : AutomatonCardModel(0, CardType.Skill, CardRarity.Common, TargetType.Self, false, false);