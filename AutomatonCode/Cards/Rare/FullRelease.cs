using Automaton.AutomatonCode.Core;
using Automaton.AutomatonCode.Interfaces;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace Automaton.AutomatonCode.Cards.Rare;

[Pool(typeof(AutomatonCardPool))]
public class FullRelease()
    : AutomatonCardModel(1, CardType.Skill, CardRarity.Rare, TargetType.Self), IEncodable, ICompilable;

// Todo: make it not hover tip with encode