using Automaton.AutomatonCode.Core;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace Automaton.AutomatonCode.Cards.Removed;

[Obsolete]
[Pool(typeof(AutomatonCardPool))]
public class GetLatest() : AutomatonCardModel(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self, false, false);