using BaseLib.Utils;
using Downfall.Code.Cards.CardModels;
using Downfall.Code.Character.Automaton;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace Downfall.Code.Cards.Automaton.Rare;

[Pool(typeof(AutomatonCardPool))]
public class FullRelease() : AutomatonCardModel(1, CardType.Skill, CardRarity.Rare, TargetType.Self), IEncodable, ICompilable;