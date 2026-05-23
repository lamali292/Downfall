using Automaton.AutomatonCode.Core;
using BaseLib.Utils;
using Downfall.DownfallCode.CustomEnums;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Automaton.AutomatonCode.Cards.Removed;

[Obsolete]
[Pool(typeof(AutomatonCardPool))]
public class Cleanse() : AutomatonCardModel(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, false, false);