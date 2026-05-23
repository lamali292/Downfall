using Automaton.AutomatonCode.Core;
using Automaton.AutomatonCode.Interfaces;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Automaton.AutomatonCode.Cards.Removed;

[Obsolete]
[Pool(typeof(AutomatonCardPool))]
public class DelayedGuard() : AutomatonCardModel(0, CardType.Skill, CardRarity.Common, TargetType.Self, false, false);