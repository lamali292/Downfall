using Automaton.AutomatonCode.Core;
using Automaton.AutomatonCode.CustomEnums;
using Automaton.AutomatonCode.Interfaces;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Automaton.AutomatonCode.Cards.Removed;

[Obsolete]
[Pool(typeof(AutomatonCardPool))]
public class GetLatest() : AutomatonCardModel(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self, false, false);